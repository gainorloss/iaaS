using Dev.ConsoleApp.DynamicQuery;
using Dev.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dev.Core
{
    [Query(nameof(Order))]
    class OrderQueryModel
    {
        public int Id { get; set; }
        public string oaid { get; set; }
    }

    [Query("Order.Id=OrderCustomer.OrderId")]
    public class OrderCustomerQueryModel
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// 内部编码
        /// </summary>
        [Display(Name = "内部编码")]
        public string OrderCode { get; set; }
        /// <summary>
        /// 店铺Id
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        /// <remarks>2018.10.14 添加</remarks>
        public string ShopName { get; set; }
        /// <summary>
        /// 网店平台
        /// </summary>
        public int WebPlatform { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public int OrderType { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        [Display(Name = "内部编码")]
        public string OrderNumber { get; set; }
        /// <summary>
        /// 买家昵称
        /// </summary>
        public string BuyerNick { get; set; }
        /// <summary>
        /// 买家备注
        /// </summary>
        public string BuyerMessage { get; set; }

        /// 预分配快递公司
        public string ExpName { get; set; }
        /// <summary>
        /// 快递规则
        /// </summary>
        public int? ExpRuleId { get; set; } // 2018.12.08 添加 快递规则Id
        /// <summary>
        /// 店铺备注
        /// </summary>
        public string ShopMessage { get; set; }
        /// <summary>
        /// 网店订单状态
        /// </summary>
        public string WebStatus { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// 物流状态
        /// </summary>
        public int OrderState { get; set; }
        /// <summary>
        /// 处理状态
        /// </summary>
        public int OperateState { get; set; }
        /// <summary>
        /// 客服备注
        /// </summary>
        public string ServiceMemo { get; set; }
        /// <summary>
        /// 操作备注
        /// </summary>
        public string OperateMemo { get; set; }
        /// <summary>
        /// 是否锁单（不拆单）
        /// </summary>
        public bool? IsMerge { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public int Kind { get; set; }
        /// <summary>
        /// 创建类型
        /// </summary>
        /// <remarks>2018.12.06 添加</remarks>
        public short? CreateKind { get; set; }
        /// <summary>
        /// 订单来源
        /// </summary>
        /// <remarks>2018.12.06 添加</remarks>
        public string OrderSource { get; set; }
        /// <summary>
        /// 预计发货日期
        /// </summary>
        /// <remarks>2018.12.13 添加</remarks>
        public DateTime? DeliveryDate { get; set; }
        /// <summary>
        /// 店铺订单Id
        /// </summary>
        public string WebOrderID { get; set; }
        /// <summary>
        /// 仓库备注
        /// </summary>
        public string StoreMemo { get; set; }

        public string oaid { get; set; }
        [Column("OrderCustomer.Name")]
        public string Name { get; set; }
        [Column("OrderCustomer.Tel")]
        public string Tel { get; set; }
        [Column("OrderCustomer.Mobile")]
        public string Mobile { get; set; }
        [Column("OrderCustomer.Address")]
        public string Address { get; set; }
        [Column("OrderCustomer.Province")]
        public string Province { get; set; }
        [Column("OrderCustomer.City")]
        public string City { get; set; }
        [Column("OrderCustomer.County")]
        public string County { get; set; }
        [Column("OrderCustomer.PostCode")]
        public string PostCode { get; set; }
        [Column("OrderCustomer.OrderAddress")]
        public string OrderAddress { get; set; }
    }

    [Query("OrderDetail.OrderId=OrderCustomerQueryModel.Id")]
    public class OrderDetailQueryModel
    {
        [Column("OrderCustomerQueryModel.OrderNumber")]
        public string OrderNumber { get; set; }
        public int Id { get; set; }
        public string WebItemId { get; set; }
        //[StringLength](40)]
        public string WebOrderId { get; set; }

        public int? ShopWareId { get; set; } // 2018.12.06 新增 店铺商品Id
        public int? Weight { get; set; } // 2018.12.06 新增 重量
        // 2019.02.02 弃用该字段 public Guid? ProductId { get; set; } // 2018.10.14 新增
        public string WareName { get; set; } // 2018.10.14 新增
        public string BarCode { get; set; } // 2018.10.14 新增
        public decimal? Discount { get; set; } // 2018.10.14 新增
        public decimal? CostPrice { get; set; }
    }
}
