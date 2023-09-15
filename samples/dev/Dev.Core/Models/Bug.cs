using Stateless;
using System.Diagnostics;

namespace Dev.Core.Models
{
    internal class Bug
    {
        private string _title;
        private string _description;
        private StateMachine<BugState, BugTrigger> _machine;
        private StateMachine<BugState, BugTrigger>.TriggerWithParameters<string> _assignTrigger;
        private string _assignee;

        public Bug(string title, string description)
        {
            _title = title;
            _description = description;

            _machine = new StateMachine<BugState, BugTrigger>(BugState.Open);
            _assignTrigger= _machine.SetTriggerParameters<string>(BugTrigger.Assign);

            _machine.Configure(BugState.Open)
                .Permit(BugTrigger.Assign, BugState.Assigned);

            _machine.Configure(BugState.Assigned)
                .SubstateOf(BugState.Open)
                .OnEntryFrom(_assignTrigger, assignee => { _assignee = assignee; System.Console.WriteLine($"Assign to {assignee}"); })
                .PermitReentry(BugTrigger.Assign)
                .Permit(BugTrigger.Defer, BugState.Deferred)
                .Permit(BugTrigger.Close, BugState.Closed)
                .OnExit(() => { });

            _machine.Configure(BugState.Deferred)
                .OnEntry(() => _assignee = null)
                .Permit(BugTrigger.Assign, BugState.Assigned);
        }

        public void Assign(string assignee)
        {
            _machine.Fire(_assignTrigger,assignee);
            Trace.WriteLine($"State machine>\t assign to {_assignee}");
            Trace.WriteLine($"State machine>\t{_machine.State.ToString()}");
        }

        public void Defer()
        {
            _machine.Fire(BugTrigger.Defer);
            Trace.WriteLine($"State machine>\t{_machine.State.ToString()}");
        }

        public void Close()
        {
            _machine.Fire(BugTrigger.Close);
            Trace.WriteLine($"State machine>\t{_machine.State.ToString()}");
        }

    }

    internal enum BugState
    {
        Open,
        Assigned,
        Deferred,
        Closed
    }

    internal enum BugTrigger
    {
        Open,
        Assign,
        Defer,
        Close
    }
}