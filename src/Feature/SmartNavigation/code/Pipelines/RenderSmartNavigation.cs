using System.Linq;
using System.Web.UI;
using Feature.SmartNavigation.Models;
using Feature.SmartNavigation.Services;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Shell.Applications.ContentEditor.Pipelines.RenderContentEditor;

namespace Feature.SmartNavigation.Pipelines
{
    public class RenderSmartNavigation
    {
        private readonly ISmartNavigationService smartNavigationService;
        private const string SectionName = "Smart Navigation";
        private const string HeaderIconPath = "/~/icon/office/16x16/lightbulb_on.png";
        private const string IconPath = "/-/icon/Office/16x16/navigate_right.png.aspx";

        public RenderSmartNavigation(ISmartNavigationService smartNavigationService)
        {
            this.smartNavigationService = smartNavigationService;
        }

        private Item CurrentItem { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Is checked by Assert")]
        public void Process(RenderContentEditorArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var parentControl = args.Parent;
            var editorFormatter = args.EditorFormatter;
            CurrentItem = args.Item;

            if (CurrentItem == null)
            {
                return;
            }

            var suggestedItems = smartNavigationService.GetSuggestions(CurrentItem, 5);

            if (!suggestedItems.NavigationsFromItem.Any())
            {
                return;
            }

            editorFormatter.RenderSectionBegin(parentControl, SectionName, SectionName, SectionName, HeaderIconPath, false, true);
            RenderNavigation(editorFormatter, parentControl, suggestedItems);
            editorFormatter.RenderSectionEnd(parentControl, true, false);
        }

        private static void RenderNavigation(EditorFormatter editorFormatter, Control parentControl, SuggestionSet suggestions)
        {
            var fromItemsOutput = string.Join(string.Empty, suggestions.NavigationsFromItem.Select(GetItemLink));
            var toItemsOutput = string.Join(string.Empty, suggestions.NavigationsToItem.Select(GetItemLink));
            var lastItemOutput = suggestions.LastItem != null ? $"<hr style=\"margin:10px 0\"/>\r\n                <div style=\"font-size: 14px; margin-bottom: 10px;\">\r\n                    <b>The last item you have worked on</b> \r\n                </div>\r\n                <div style=\"padding-left: 7px;\"><a href=\"#\" class=\"scLink\" title=\"{suggestions.LastItem.Path}\"\r\n                    onclick=\"javascript:return scForm.invoke(&quot;item:load(id={suggestions.LastItem.ItemId},language=en,version=1)&quot;)\">\r\n                    <span><img src=\"/-/icon/Office/16x16/navigate_left.png.aspx\" width=\"16\" height=\"16\" class=\"scContentTreeNodeIcon\" alt=\"\" border=\"0\"><b>{suggestions.LastItem.Name}</b> - [{suggestions.LastItem.Path}]</span></a></div>\r\n            " : string.Empty;
            var output = $"<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"scEditorFieldMarker\">\r\n    <tbody>\r\n        <tr>\r\n            <td id=\"FieldMarkerFIELD2123021\" class=\"scEditorFieldMarkerBarCell\"><img src=\"/sitecore/images/blank.gif\"\r\n                    width=\"4px\" height=\"1px\"></td>\r\n            <td class=\"scEditorFieldMarkerInputCell\">\r\n                <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">\r\n                    <tbody>\r\n                        <tr>\r\n                            <td>\r\n                                <div style=\"font-size: 14px; margin-bottom: 10px;\"><b>You might want to work on these next</b></div>\r\n                                <ul style=\"padding-left:30px; list-style-image: url('/-/icon/Office/16x16/navigate_right.png.aspx');\">\r\n                                    {fromItemsOutput}\r\n                                </ul>\r\n                            </td>\r\n                            <td>\r\n                                <div style=\"font-size: 14px; margin-bottom: 10px;\"><b>You might want to go back to these</b></div>\r\n                                <ul style=\"padding-left:30px; list-style-image: url('/-/icon/Office/16x16/navigate_right.png.aspx');\">\r\n                                    {toItemsOutput}\r\n                                </ul>\r\n                            </td>\r\n                        </tr>\r\n                    </tbody>\r\n                </table>\r\n                {lastItemOutput}</td>\r\n        </tr>\r\n    </tbody>\r\n</table>";
            editorFormatter.AddLiteralControl(parentControl, output);
        }

        private static string GetItemLink(NavigationItem item) =>
            $"<li><a href=\"#\" class=\"scLink\" title=\"{item.Path}\"\r\n    onclick=\"javascript:return scForm.invoke(&quot;item:load(id={item.ItemId},language=en,version=1)&quot;)\"><span\r\nstyle=\"top:-4px; position:relative;\"><b>{item.Name}</b> -\r\n[{item.Path}]</span></a></li>";
    }
}