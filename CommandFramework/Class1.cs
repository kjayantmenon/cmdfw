using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandFramework
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class AbstractCommand:ICommand
    {
        public async Task Queue()
        {
            AddCommandToQueue();
            SendCommand();
        }

        private async  Task SendCommand()
        {
            throw new NotImplementedException();
        }

        private void AddCommandToQueue()
        {
            throw new NotImplementedException();
        }
    }

    public interface ICommand
    {
    }
}
