using CommunityToolkit.Mvvm.Input;
using SOCMobile.Models;

namespace SOCMobile.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}