using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Web.UI.Sheer;
using System;
namespace Sitecore.Support.Shell.Framework.Pipelines
{
    public class NewFolderItem
    {
        public void Execute(ClientPipelineArgs args)
        {
            if (args.HasResult)
            {
                Database database = Factory.GetDatabase(args.Parameters["database"]);
                Assert.IsNotNull(database, "database");
                string text = args.Parameters["language"];
                string itemPath = args.Parameters["id"];
                string @string = StringUtil.GetString(new string[]
                {
                    args.Parameters["master"]
                });
                Item item = (text == null) ? database.Items[itemPath] : database.Items[itemPath, Language.Parse(text)];
                if (item != null)
                {
                    TemplateItem templateItem = (@string.Length > 0) ? database.Templates[@string] : database.Templates[TemplateIDs.Folder];
                    if (templateItem != null)
                    {
                        Log.Audit(this, "Create folder: {0}", new string[]
                        {
                            AuditFormatter.FormatItem(item)
                        });
                        templateItem.AddTo(item, args.Result);
                        return;
                    }
                    Context.ClientPage.ClientResponse.Alert(Translate.Text("Neither {0} master or {1} template was found.", new object[]
                    {
                        @string,
                        @string
                    }));
                    args.AbortPipeline();
                    return;
                }
                else
                {
                    Context.ClientPage.ClientResponse.Alert("Item not found.");
                    args.AbortPipeline();
                }
            }
        }
    }
}
