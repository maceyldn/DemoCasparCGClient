using System;
using System.Collections.Generic;
using System.Text;

namespace Svt.Caspar
{
	public class TemplatesCollection
	{
		Dictionary<string, List<TemplateInfo>> templates_ = new Dictionary<string, List<TemplateInfo>>();
		List<TemplateInfo> all_ = new List<TemplateInfo>();

		internal TemplatesCollection()
		{ }

		internal void Populate(List<TemplateInfo> templates)
		{
			Dictionary<string, List<TemplateInfo>> newTemplates = new Dictionary<string, List<TemplateInfo>>();
			List<TemplateInfo> newAll = new List<TemplateInfo>();

			foreach (TemplateInfo template in templates)
			{
				string key = template.Folder;
				if (!newTemplates.ContainsKey(key))
					newTemplates.Add(key, new List<TemplateInfo>());

				newTemplates[key].Add(template);
				newAll.Add(template);
			}

			System.Threading.Interlocked.Exchange<Dictionary<string, List<TemplateInfo>>>(ref templates_, newTemplates);
			System.Threading.Interlocked.Exchange<List<TemplateInfo>>(ref all_, newAll);			
		}

		public List<TemplateInfo> GetTemplatesInFolder(string folder)
		{
			return templates_[folder];
		}

		public List<TemplateInfo> All { get { return all_; } }

		public ICollection<string> Folders
		{
			get { return templates_.Keys; }
		}

		internal void Clear()
		{
			templates_.Clear();
		}
	}
}
