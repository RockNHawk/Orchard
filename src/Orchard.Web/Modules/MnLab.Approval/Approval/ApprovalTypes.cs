using System;
using System.Collections.Generic;

namespace MnLab.Enterprise.Approval {
    /// <summary>
    /// 内容的操作类型：新增、编辑、删除
    /// </summary>
    public class ApprovalType {
        public virtual Dictionary<string, object> LockedProperties { get; set; }

        //= new Dictionary<string, bool>()
        //{
        //    {"Value.Name",true},
        //};

        public bool IsLocked(string propertyName) {
            var ps = LockedProperties;
            return ps == null ? false : ps.ContainsKey(propertyName);
        }


        /// <summary>
        /// 默认
        /// </summary>
        public static readonly System.Type None = new NoneApprovalType().GetType();
        /// <summary>
        /// 新增审批
        /// </summary>
        public static readonly System.Type Creation = new CreationApprovalType().GetType();
        /// <summary>
        /// 修改审批
        /// </summary>
        public static readonly System.Type Modification = typeof(ModificationApprovalType);
        /// <summary>
        /// 删除审批
        /// </summary>
        public static readonly System.Type Deletion = new DeletionApprovalType().GetType();
        ///// <summary>
        ///// 还款完成审批
        ///// </summary>
        //public static readonly System.Type PayOffFinish = new PayOffFinishApprovalType().GetType();

        static readonly Dictionary<System.Type, ApprovalType> approvalTypes = new Dictionary<Type, ApprovalType>();

        public static ApprovalType Of(System.Type approvalType) {
            ApprovalType value;
            if (!approvalTypes.TryGetValue(approvalType, out value)) {
                value = (ApprovalType)Activator.CreateInstance(approvalType);
                approvalTypes[approvalType] = value;
            }
            return value;
        }

        public static ApprovalType Of(System.Type approvalType, System.Type contentType) {
            if (approvalType == null) {
                throw new ArgumentNullException(nameof(approvalType));
            }
            if (contentType == null) {
                throw new ArgumentNullException(nameof(contentType));
            }
            var key = typeof(Tuple<,>).MakeGenericType(approvalType, contentType);
            ApprovalType value;
            if (!approvalTypes.TryGetValue(key, out value)) {
                return null;
            }
            return value;
        }

        public static ApprovalType Of<T>() where T : ApprovalType {
            return Of(typeof(T));
        }


        public static void Set(System.Type approvalType, System.Type contentType, ApprovalType at) {
            approvalTypes[typeof(Tuple<,>).MakeGenericType(approvalType, contentType)] = at;
        }

        bool m_IsDirectEditable = true;
        /// <summary>
        /// 是否允许直接编辑所有字段
        /// </summary>
        public virtual bool IsDirectEditable { get { return m_IsDirectEditable; } set { m_IsDirectEditable = value; } }

    }
}