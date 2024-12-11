#!/bin/bash

if [ $# -eq 0 ]; then
    echo "Usage: $0 <build_target_directory_path>"
    exit 1
fi
build_target_path="$1"
output_file="app_components.xml"

if [ -e "$output_file" ]; then
    rm "$output_file"
fi

for file_in_build_directory in "$build_target_path"/*; do
    if [ -f "$file_in_build_directory" ] && [[ "$file_in_build_directory" == *.dll ]]; then
        filename=$(basename "$file_in_build_directory")
        if [ "$filename" != "EAModelKit.dll" ]; then
            echo $filename
            file_content=$(cat <<EOF
<Component>
    <File Source="${filename}" />
</Component>
EOF
)
            echo "$file_content" >> "$output_file"
        fi
    fi
done

echo "export over"
