namespace Galosoft.IaaS.Core
{
    public class NameValue
    {
        public NameValue()
        { }

        public NameValue(
            string name,
            object value,
            string ramark)
        {
            Name = name;
            Value = value;
            Remark = ramark;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
