// -------------------------------------------------------------------------------------------------
// <copyright file="TestCollection.cs" company="Starion Group S.A.">
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

namespace EAModelKit.Tests.Helpers
{
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;

    using EA;

    using Moq;

    /// <summary>
    /// Class that is there to facilitate unit tests when touching <see cref="Collection"/>, since all constructors for all EA classes are internal
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TestCollection : Collection
    {
        /// <summary>
        /// A containedObjects of the contained object
        /// </summary>
        private readonly List<object> containedObjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCollection" /> class.
        /// </summary>
        public TestCollection()
        {
            this.containedObjects = [];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCollection" /> class.
        /// </summary>
        /// <param name="objects">A collection of <see cref="object"/></param>
        public TestCollection(IEnumerable<object> objects)
        {
            this.containedObjects = objects.ToList();
        }

        /// <summary>
        /// Gets the number of contained objects
        /// </summary>
        public short Count => (short)this.containedObjects.Count;

        /// <summary>
        /// Gets the <see cref="ObjectType" /> of contained objects
        /// </summary>
        public ObjectType ObjectType => 0;

        /// <summary>
        /// Retrieves a contained object at the given index
        /// </summary>
        /// <param name="index">The index of the object</param>
        /// <returns>The contained object</returns>
        public object GetAt(short index)
        {
            return this.IsIndexInRange(index) ? this.containedObjects[index] : null;
        }

        /// <summary>
        /// Removes a contained object
        /// </summary>
        /// <param name="index">The index of the object</param>
        /// <param name="refresh">Not used</param>
        public void DeleteAt(short index, bool refresh)
        {
            if (!this.IsIndexInRange(index))
            {
                return;
            }

            this.containedObjects.RemoveAt(index);
        }

        /// <summary>
        /// Gets the last occured error (Not used)
        /// </summary>
        /// <returns>null</returns>
        public string GetLastError()
        {
            return null;
        }

        /// <summary>
        /// Gets a object by his name (Not used)
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <returns>null</returns>
        public object GetByName(string name)
        {
            foreach (var containedObject in this.containedObjects)
            {
                if (containedObject is TaggedValue taggedValue && taggedValue.Name == name)
                {
                    return taggedValue;
                }
            }

            return null;
        }

        /// <summary>
        /// Refresh the collection (Not used)
        /// </summary>
        public void Refresh()
        {
        }

        /// <summary>
        /// Adds a new object to the contained objects
        /// </summary>
        /// <param name="Name">The name of the object</param>
        /// <param name="Type">The Type if the object</param>
        /// <returns>The created object</returns>
        public object AddNew(string Name, string Type)
        {
            if (Type == nameof(TaggedValue))
            {
                var newTaggedValue = new Mock<TaggedValue>();
                newTaggedValue.Setup(x => x.Name).Returns(Name);
                this.Add(newTaggedValue.Object);
                return newTaggedValue.Object;
            }

            return null;
        }

        /// <summary>
        /// Removes a contained object
        /// </summary>
        /// <param name="index">The index of the objects to removed</param>
        public void Delete(short index)
        {
            this.DeleteAt(index, false);
        }

        /// <summary>
        /// Gets the <see cref="IEnumerator" /> to iterate through the collection
        /// </summary>
        /// <returns>The <see cref="IEnumerator" /></returns>
        public IEnumerator GetEnumerator()
        {
            return this.containedObjects.GetEnumerator();
        }

        /// <summary>
        /// Adds an object to the contained Object
        /// </summary>
        /// <param name="newObject">The new object</param>
        public void Add(object newObject)
        {
            this.containedObjects.Add(newObject);
        }

        /// <summary>
        /// Adds a collection of object
        /// </summary>
        /// <param name="objects">The collection to add</param>
        public void AddRange(IEnumerable<object> objects)
        {
            this.containedObjects.AddRange(objects);
        }

        /// <summary>
        /// Asserts if the given index is in range of the <see cref="containedObjects" /> collection
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>Asserts that the index is in range</returns>
        private bool IsIndexInRange(short index)
        {
            return index >= 0 && index < this.containedObjects.Count;
        }
    }
}
