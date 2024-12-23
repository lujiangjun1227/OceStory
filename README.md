# OceCommon Library

## Overview
The `OceCommon` library provides a utility function to compute the intersection of two JSON-encoded lists of objects based on specified attribute mappings. This is particularly useful for scenarios where data from two lists need to be matched on specific properties, such as data synchronization or reconciliation tasks.

## Features
- Compare two lists and find records with matching attributes.
- Output both the intersection and non-intersection records.
- Supports parallel processing for performance optimization.
- Configurable batch size for processing large datasets.

## Requirements
- .NET Framework 4.7.2 or higher / .NET Core 3.1 or higher
- Newtonsoft.Json package

## Installation
Clone this repository or copy the `OceCommon` class into your project. Ensure the `Newtonsoft.Json` package is installed:

```bash
Install-Package Newtonsoft.Json
```

## Usage

### Method: `GetIntersection`

#### Parameters
- `ActListJson` (string): JSON representation of the base list.
- `TargetListJson` (string): JSON representation of the target list.
- `AttributeList` (List of `ListCompareAttribure`): List of attribute mappings for comparison.
- `out IntersectionListJson` (string): JSON representation of the intersecting records.
- `out NonIntersectionListJson` (string): JSON representation of the non-intersecting records.
- `out IsSuccess` (bool): Indicates whether the operation was successful.
- `out ErrMsg` (string): Error message in case of failure.

#### Example

```csharp
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OceStory
{
    class Program
    {
        static void Main(string[] args)
        {
            var actListJson = "[{'Id':1,'Name':'Alice'},{'Id':2,'Name':'Bob'}]";
            var targetListJson = "[{'Id':1,'Name':'Alice'},{'Id':3,'Name':'Charlie'}]";

            var attributeList = new List<ListCompareAttribure>
            {
                new ListCompareAttribure { ActAttributeName = "Id", TarAttributeName = "Id" },
                new ListCompareAttribure { ActAttributeName = "Name", TarAttributeName = "Name" }
            };

            var oceCommon = new OceCommon();
            oceCommon.GetIntersection(
                actListJson,
                targetListJson,
                attributeList,
                out var intersectionListJson,
                out var nonIntersectionListJson,
                out var isSuccess,
                out var errMsg
            );

            if (isSuccess)
            {
                Console.WriteLine("Intersection:");
                Console.WriteLine(intersectionListJson);
                Console.WriteLine("\nNon-Intersection:");
                Console.WriteLine(nonIntersectionListJson);
            }
            else
            {
                Console.WriteLine($"Error: {errMsg}");
            }
        }
    }
}
```

### Class: `ListCompareAttribure`

#### Definition
```csharp
public class ListCompareAttribure
{
    public string ActAttributeName { get; set; } // Attribute name in the base list
    public string TarAttributeName { get; set; } // Attribute name in the target list
}
```

#### Example Usage
```csharp
var attributeList = new List<ListCompareAttribure>
{
    new ListCompareAttribure { ActAttributeName = "Id", TarAttributeName = "Id" },
    new ListCompareAttribure { ActAttributeName = "Name", TarAttributeName = "Name" }
};
```

## Key Functionality
### Parallel Processing
The method leverages `Parallel.For` to process records in batches, enabling efficient handling of large datasets. The batch size and degree of parallelism can be adjusted for optimal performance.

### Error Handling
The method provides detailed error messages in case of invalid inputs or unexpected issues, ensuring robust operation.

## Notes
- Ensure the JSON input strings are correctly formatted.
- Attributes specified in `AttributeList` must exist in both the base and target lists.
- The function creates unique keys for comparison by concatenating attribute values.

## Contributing
Feel free to fork this repository, make improvements, and submit pull requests. Contributions are always welcome!

## License
This project is licensed under the MIT License.

---

Enjoy using the `OceCommon` library for efficient list comparisons!

