// -------------------------------------------------------------------------------------------------
// <copyright file="ExcelWriter.cs" company="Starion Group S.A.">
// 
//     Copyright (C) 2024 Starion Group S.A.
// 
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
// 
//         http://www.apache.org/licenses/LICENSE-2.0
// 
//     Unless required by applicable law or agreed to in writing, software
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
// 
// </copyright>
// -----------------------------------------------------------------------------------------------

namespace EAModelKit.Services.Writer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using ClosedXML.Excel;

    using EAModelKit.Model.Export;

    /// <summary>
    /// The <see cref="ExcelWriter" /> provides writting features to Excel format
    /// </summary>
    internal class ExcelWriter : IExcelWriter
    {
        /// <summary>
        /// Writes the content of the dictionary into an Excel file, at the given <paramref name="filePath" /> location
        /// </summary>
        /// <param name="exportableObjectsContent">The Dictionary that contains all information that should be exported.</param>
        /// <param name="filePath">The export file path</param>
        /// <returns>A <see cref="Task" /></returns>
        public Task WriteAsync(IReadOnlyDictionary<string, IReadOnlyList<ExportableObject>> exportableObjectsContent, string filePath)
        {
            return Task.Run(() =>
            {
                using var workBook = new XLWorkbook();

                foreach (var exportableObjects in exportableObjectsContent)
                {
                    CreateExcelWorkSheet(workBook, exportableObjects.Key, exportableObjects.Value);
                }

                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                workBook.SaveAs(fileStream);
            });
        }

        /// <summary>
        /// </summary>
        /// <param name="workBook"></param>
        /// <param name="workSheetName"></param>
        /// <param name="exportableObjects"></param>
        private static void CreateExcelWorkSheet(IXLWorkbook workBook, string workSheetName, IReadOnlyList<ExportableObject> exportableObjects)
        {
            var headerContent = exportableObjects[0].Headers;
            var workSheet = workBook.AddWorksheet(workSheetName.Replace("/", ""));

            WriteHeader(workSheet, headerContent);
            WorkSheetContent(workSheet, headerContent, exportableObjects);

            const double minWidth = 20;
            const double maxWidth = 70;
            
            foreach (var item in workSheet.ColumnsUsed())
            {
                item.AdjustToContents(minWidth, maxWidth);
            }
        }

        /// <summary>
        /// Writes the content of all <see cref="ExportableObject" /> into a <see cref="IXLWorksheet" />
        /// </summary>
        /// <param name="workSheet">The <see cref="IXLWorksheet" /></param>
        /// <param name="headerContent">The header content</param>
        /// <param name="exportableObjects">The collection of <see cref="ExportableObject" /> that have to be written</param>
        private static void WorkSheetContent(IXLWorksheet workSheet, IReadOnlyList<string> headerContent, IReadOnlyList<ExportableObject> exportableObjects)
        {
            var rowIndex = 2;

            foreach (var exportableObject in exportableObjects)
            {
                var row = workSheet.Row(rowIndex++);
                row.Style.Alignment.WrapText = true;

                for (var columnIndex = 1; columnIndex <= headerContent.Count; columnIndex++)
                {
                    var cell = row.Cell(columnIndex);
                    cell.Value = exportableObject[headerContent[columnIndex - 1]];
                }
            }
        }

        /// <summary>
        /// Writes the header (first row) of a <see cref="IXLWorksheet" /> with the <paramref name="headerContent" />
        /// </summary>
        /// <param name="workSheet">The <see cref="IXLWorksheet" /></param>
        /// <param name="headerContent">The content of the header</param>
        private static void WriteHeader(IXLWorksheet workSheet, IReadOnlyList<string> headerContent)
        {
            var headerRow = workSheet.Row(1);
            headerRow.Style.Font.Bold = true;
            workSheet.SheetView.FreezeRows(1);

            for (var headerIndex = 1; headerIndex <= headerContent.Count; headerIndex++)
            {
                var cell = headerRow.Cell(headerIndex);
                cell.Value = headerContent[headerIndex - 1];
            }
        }
    }
}
