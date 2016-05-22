using SlavaGu.ConsoleAppLauncher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCmd.Models.CommandPrompt;

namespace WebCmd.Services
{
    public class CommandPromptService
    {
        private static Dictionary<string, Tuple<ConsoleApp, StringBuilder>> _userCommand;

        static CommandPromptService()
        {
            _userCommand = new Dictionary<string, Tuple<ConsoleApp, StringBuilder>>();
        }

        public string RunCommand(string userId, CommandInfoVM commandInfoVM)
        {
            SaveCommand(userId, commandInfoVM);
            RegisterEvent(userId);

            var timeStart = Run(userId);
            CheckState(userId);
            if (_userCommand[userId].Item1.ExitTime.HasValue)
            {
                var timeSpent = _userCommand[userId].Item1.ExitTime.Value - timeStart;
                return $"{_userCommand[userId].Item2} \nTime spent: {timeSpent}";
            }

            return "You should never see this code :)";
        }

        public string CancelCommand(string userId, CommandInfoVM commandInfoVM)
        {
            if (_userCommand[userId].Item1.State == AppState.Exiting || _userCommand[userId].Item1.State == AppState.Running)
            {
                _userCommand[userId].Item1.Stop();
                return
                    $"Command {_userCommand[userId].Item1.CmdLine.Substring(3, _userCommand[userId].Item1.CmdLine.Length - 3)} was successfully closed.";
            }
            else
            {
                throw new InvalidOperationException("It seems, that command had finished before you cancelled it.");
            }
        }

        private DateTime Run(string userId)
        {
            var timeStart = DateTime.Now;
            _userCommand[userId].Item1.Run();
            _userCommand[userId].Item1.WaitForExit();
            return timeStart;
        }

        private void SaveCommand(string userId, CommandInfoVM commandInfoVM)
        {
            var command = $"/c {commandInfoVM.Command} {commandInfoVM.Path}";
            CheckState(userId);
            SetCommandForUser(userId, command);

        }

        private void SetCommandForUser(string userId, string command)
        {
            if (!_userCommand.ContainsKey(userId))
                _userCommand.Add(userId, new Tuple<ConsoleApp, StringBuilder>(
                                                new ConsoleApp("cmd", command),
                                                new StringBuilder()));
            else
                _userCommand[userId] = new Tuple<ConsoleApp, StringBuilder>(
                                                new ConsoleApp("cmd", command),
                                                _userCommand[userId].Item2.Clear());
        }

        private void CheckState(string userId)
        {
            if (_userCommand.ContainsKey(userId))
                if (_userCommand[userId].Item1.State == AppState.Running)
                    throw new InvalidOperationException("Console is runnting");
                else if (_userCommand[userId].Item1.State == AppState.Exiting)
                    throw new InvalidOperationException("Console is exiting");
        }

        private void RegisterEvent(string userId)
        {
            _userCommand[userId].Item1.ConsoleOutput += ((o, e) =>
            {
                _userCommand[userId].Item2.AppendLine(e.Line);
            });
        }
    }
}
