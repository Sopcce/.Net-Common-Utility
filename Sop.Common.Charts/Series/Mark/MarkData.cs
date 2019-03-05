﻿

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Sop.Common.Charts.Series.Mark
{
    /// <summary>
    ///     标注的数据数组。每个数组项是一个对象，有下面几种方式指定标注的位置。
    /// </summary>
    public class MarkData
    {
        /// <summary>
        ///     标注名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     特殊的标注类型，用于标注最大值最小值等。
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MarkPointDataTypes Type { get; set; }

        /// <summary>
        ///     标注的坐标。坐标格式视系列的坐标系而定，可以是直角坐标系上的 x, y，也可以是极坐标系上的 radius, angle。
        ///     例如 [121, 2323]、['aa', 998]。
        /// </summary>
        public List<object> Coord { get; set; }
    }
}