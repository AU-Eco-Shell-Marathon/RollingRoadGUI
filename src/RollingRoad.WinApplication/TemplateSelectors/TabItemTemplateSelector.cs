using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RollingRoad.WinApplication.ViewModels;
using ViewModelTemplateTuple = System.Tuple<System.Type, string>;

namespace RollingRoad.WinApplication
{
    public class TabItemTemplateSelector : DataTemplateSelector
    {
        private readonly List<ViewModelTemplateTuple> _viewModelTemplateList = new List<ViewModelTemplateTuple>()
        {
            new ViewModelTemplateTuple(typeof(LiveDataSourceViewModel), "LiveDataTemplate"),
            new ViewModelTemplateTuple(typeof(DataSetsViewModel), "ViewTemplate"),
            new ViewModelTemplateTuple(typeof(LoggerViewModel), "LoggerTemplate")}; 

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element == null || item == null)
                return null;

            ViewModelTemplateTuple template = _viewModelTemplateList.FirstOrDefault(x => x.Item1 == item.GetType());

            if(template != null)
                return element.FindResource(template.Item2) as DataTemplate;

            return null;
        }
    }
}
