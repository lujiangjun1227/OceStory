using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static OceStory.OceCommonStruc;

namespace OceStory
{
    public class OceCommon
    {
        /// <summary>
        /// 以ActList为基准，查找在TargetList中对比部分属性相同记录的交集,AttributeList传入需比较的属性
        /// </summary>
        /// <param name="ActListJson">基准List的Json</param>
        /// <param name="TargetListJson">目标Lis的Json</param>
        /// <param name="AttributeList">求交集对应的对比属性</param>
        /// <param name="IntersectionListJson">基于ActList获得的与TargetList中部分属性相同的交集</param>
        /// <param name="NonIntersectionListJson">非交集数据</param>
        /// <param name="IsSuccess">是否成功标记</param>
        /// <param name="ErrMsg">错误信息</param>
        public void GetIntersection(string ActListJson, string TargetListJson, List<ListCompareAttribure> AttributeList, out string IntersectionListJson, out string NonIntersectionListJson, out bool IsSuccess, out string ErrMsg)
        {

            IntersectionListJson = "";
            NonIntersectionListJson = "";
            IsSuccess = false;
            ErrMsg = "";

            try
            {
                var actList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(ActListJson);
                var targetList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(TargetListJson);

                if (actList == null || targetList == null || AttributeList == null)
                    throw new ArgumentException("输入列表或属性列表不能为空。");

                // 创建属性映射
                var propertyMapping = new Dictionary<string, string>();
                for (int i = 0; i < AttributeList.Count; i++)
                {
                    propertyMapping.Add(AttributeList[i].ActAttributeName, AttributeList[i].TarAttributeName);
                }

                // 将目标列表转换为唯一键集合，预处理减少重复计算
                var targetSet = new HashSet<string>(targetList.Select(item => CreateKey(item, propertyMapping)));

                // 并行处理
                var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }; // 控制并行线程数
                var intersectionList = new ConcurrentBag<Dictionary<string, object>>();
                var nonIntersectionList = new ConcurrentBag<Dictionary<string, object>>();

                var batchSize = 1000; // 每批处理 1000 条记录
                Parallel.For(0, (int)Math.Ceiling((double)actList.Count / batchSize), parallelOptions, batchIndex =>
                {
                    // 获取当前批次数据
                    var actBatch = actList.Skip(batchIndex * batchSize).Take(batchSize).ToList();
                    var batchIntersection = new List<Dictionary<string, object>>();
                    var batchNonIntersection = new List<Dictionary<string, object>>();

                    foreach (var item in actBatch)
                    {
                        var key = CreateKey(item, propertyMapping);
                        if (targetSet.Contains(key))
                        {
                            batchIntersection.Add(item);
                        }
                        else
                        {
                            batchNonIntersection.Add(item);
                        }
                    }

                    // 添加结果到线程安全集合
                    foreach (var item in batchIntersection) intersectionList.Add(item);
                    foreach (var item in batchNonIntersection) nonIntersectionList.Add(item);
                });

                // 序列化结果
                IntersectionListJson = JsonConvert.SerializeObject(intersectionList);
                NonIntersectionListJson = JsonConvert.SerializeObject(nonIntersectionList);
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
        }


        /// <summary>
        /// 根据属性映射创建唯一键
        /// </summary>
        private static string CreateKey(Dictionary<string, object> dict, Dictionary<string, string> propertyMapping)
        {
            return string.Join("_", propertyMapping.Select(mapping =>
                dict.ContainsKey(mapping.Key) ? dict[mapping.Key]?.ToString() ?? "" : ""));
        }

    }
}
