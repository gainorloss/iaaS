using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dev.Entities
{
    //订单买家信息表
    [Table("T_OrderCustomers")]
    public partial class OrderCustomer
    {
        [Key]
        [Column("OrderID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OrderId { get; set; }
        [Column("BuyerNick")]
        public string NickName { get; set; }
        [Column("ReceiverName")]
        public string Name { get; set; }
        [Column("ReceiverTel")]
        public string Tel { get; set; }
        [Column("ReceiverMobile")]
        public string Mobile { get; set; }
        [Column("ReceiverAddr")]
        public string Address { get; set; }
        [Column("ReceiverState")]
        public string Province { get; set; }
        [Column("ReceiverCity")]
        public string City { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }
        public string OrderAddress { get; set; }
        public DateTime? CrateedTime { get; set; }

        // 2018.10.14
        //弃用:  [Column("AlipayID")]
        //弃用:  public string AlipayId { get; set; }
        //弃用:  public string AlipayNo { get; set; }
    }
}
