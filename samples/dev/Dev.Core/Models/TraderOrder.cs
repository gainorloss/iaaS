using Stateless;
using System.Diagnostics;

namespace Dev.Core.Models
{
    public class TraderOrder
    {
        private readonly StateMachine<TraderOrderState, TradeOrderTrigger> _machine;
        private readonly StateMachine<TraderOrderState, TradeOrderTrigger>.TriggerWithParameters<long> _payTrigger;

        public long Tid { get; set; }
        public string OrderNo { get; set; }
        public TraderOrderState OrderState { get; protected set; }

        public TraderOrder(long tid, string orderNo, TraderOrderState orderState)
        {
            Tid = tid;
            OrderNo = orderNo;
            OrderState = orderState;

            _machine = new StateMachine<TraderOrderState, TradeOrderTrigger>(OrderState);
            _payTrigger = _machine.SetTriggerParameters<long>(TradeOrderTrigger.Paid);

            _machine.Configure(TraderOrderState.Submitted)
                .Permit(TradeOrderTrigger.Paid, TraderOrderState.Paid)
                .Permit(TradeOrderTrigger.Cancel, TraderOrderState.Canceled);

            _machine.Configure(TraderOrderState.Paid)
                .OnEntryFrom(_payTrigger, money => Trace.WriteLine($"交易单>\t支付{money}"))
                .Permit(TradeOrderTrigger.Cancel, TraderOrderState.Canceled)
                .Permit(TradeOrderTrigger.Sent, TraderOrderState.Sent);
        }

        public void Pay(long money)
        {
            _machine.Fire(_payTrigger, money);
            OrderState = _machine.State;
            Trace.WriteLine($"交易单>\t支付{money}");
        }
    }

    public enum TradeOrderTrigger
    {
        Submit,
        Paid,
        Cancel,
        Sent,
    }

    public enum TraderOrderState
    {
        Submitted,
        Paid,
        Canceled,
        Sent
    }
}