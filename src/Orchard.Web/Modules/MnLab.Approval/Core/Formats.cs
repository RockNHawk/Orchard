/*************************************************
  Author:   aska li      
  CreatedDate:  2014/04/
  function:  提供 IFormattable 接口的常用的 format。
 
  Modified History:        
    1. Date: 
       Author: 
       Modification: 
*************************************************/
using System;

namespace Rhythm
{
    /// <summary>
    /// 提供 IFormattable 接口的常用的 format。
    /// </summary>
    public static class Formats
    {
        /// <summary>
        /// 用于在用户界面显示的格式
        /// </summary>
        public const string DisplayName = "DisplayName";
        /// <summary>
        /// 事件日志标题格式
        /// </summary>
        public const string EventTitle = "EventTitle";

        public static string GetDisplayName<T>(T obj) where T : IFormattable
        {
            return obj.ToString(DisplayName, null);
        }
    }
}
