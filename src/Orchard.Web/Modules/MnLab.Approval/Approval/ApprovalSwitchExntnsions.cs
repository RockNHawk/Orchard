namespace MnLab.Enterprise.Approval {
    /// <summary>
    /// 审批开关扩展方法
    /// </summary>
    public static class ApprovalSwitchExntnsions
    {
        /// <summary>
        /// 是否本级审批开关都开
        /// </summary>
        /// <param name="approvalSwitch"></param>
        /// <returns></returns>
        public static bool IsOn(this ApprovalSwitch approvalSwitch)
        {
            return ((approvalSwitch ^ ApprovalSwitch.On) != (approvalSwitch | ApprovalSwitch.On));
        }
    }

}