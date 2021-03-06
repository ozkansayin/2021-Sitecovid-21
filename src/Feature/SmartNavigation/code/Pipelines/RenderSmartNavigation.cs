using System;
using System.Collections.Generic;
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
        private const string IconPath = "-/icon/Network/16x16/link.png";

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

            var suggestedItems = smartNavigationService.GetSuggestions(CurrentItem, 5).ToList();

            editorFormatter.RenderSectionBegin(parentControl, SectionName, SectionName, SectionName, IconPath, false, true);
            RenderNavigation(editorFormatter, parentControl, suggestedItems);
            editorFormatter.RenderSectionEnd(parentControl, true, false);
        }

        private void RenderNavigation(EditorFormatter editorFormatter, Control parentControl, List<NavigationItem> items)
        {
            items = new List<NavigationItem>()
            {
                new NavigationItem()
                {
                    ItemId = new Guid("EB443C0B-F923-409E-85F3-E7893C8C30C2"),
                    Name = "Sublayouts",
                    Path = "/sitecore/layout/Sublayouts"
                },
                new NavigationItem()
                {
                    ItemId = new Guid("73BAECEB-744D-4D4A-A7A5-7A935638643F"),
                    Name = "Sample",
                    Path = "/sitecore/templates/Sample"
                }
            };

            var itemsOutput = items.Select(item => $"<li><a href=\"#\" class=\"scLink\" title=\"{item.Path}\" onclick =\"javascript:return scForm.invoke(&quot;item:load(id={{{item.ItemId}}},language=en,version=1)&quot;)\"><span style=\"top:-4px; position:relative;\">{item.Name}</span></a></li>");

            var output = $"<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"scEditorFieldMarker\"><tbody><tr><td id=\"FieldMarkerFIELD2123021\" class=\"scEditorFieldMarkerBarCell\"><img src=\"/sitecore/images/blank.gif\" width=\"4px\" height=\"1px\"></td><td class=\"scEditorFieldMarkerInputCell\"><div class=\"scEditorFieldLabel\" title=\"Suggested Items to Navigate\">Suggested Items to Navigate</div><div><ul style=\"padding-left:30px; list-style-image: url('/temp/iconcache/software/16x16/branch.png');\">" +
                         $"{string.Join("", itemsOutput)}" +
                         $"</ul></div></td></tr></tbody></table>";

            editorFormatter.AddLiteralControl(parentControl, output);
        }

    }
}