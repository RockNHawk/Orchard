using Rhythm;
//using Rhythm.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace Bitlab.Enterprise
{
    /// <summary>
    /// 部室实体
    /// </summary>
   // [TypeDisplay(Name = "部室")]
    public class DepartmentRecord // : EntityBase<HierarchyId>, IFormattable
    {
        ///// <summary>
        ///// 部室iD
        ///// </summary>
        //public virtual HierarchyId Id { get; set; }

        public virtual int Id { get; set; }

        /// <summary>
        /// 分行id
        /// </summary>
        [Display(Name = "分行id")]
        public virtual int? BranchId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "序号")]
        public virtual int SortOrder { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "部门名称")]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "请输入部门名称")]
        [System.ComponentModel.DataAnnotations.StringLength(20)]

        public virtual string Name { get; set; }

        ///// <summary>
        ///// 名称
        ///// </summary>
        //[Display(Name = "显示模式")]
        //public virtual DisplayType DisplayType { get; set; }

        /// <summary>
        /// 获取对象指定的字符串格式
        /// </summary>
        /// <param name="format">指定的格式</param>
        /// <param name="formatProvider">An System.IFormatProvider that supplies culture-specific formatting information.</param>
        /// <returns>A string representation of value of the current object asspecified by provider.</returns>
        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
            {
                return base.ToString();
            }
            switch (format)
            {
                default:
                    throw new FormatException(String.Format("The {0} format string is not supported.", format));
                //case Formats.DisplayName:
                //case Formats.EventTitle:
                    return this.Name;
            }
        }

    }
}
