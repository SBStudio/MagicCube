using System.IO;
using System.Xml;

namespace Framework
{
	public sealed class XmlUtil
	{
		private const char SPLIT = '/';
		private const string ELEMENT_ROOT = "root";

		public XmlDocument xmlDocument { get; private set; }
		public XmlElement xmlRoot { get { return xmlDocument.DocumentElement; } }

		public XmlUtil(string xml)
		{
			xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
		}

		public void Save(string path)
		{
			xmlDocument.Save(path);
		}

		public XmlElement Add(string name)
		{
			return Add(xmlRoot, name);
		}

		public XmlElement Add(XmlElement parent, string name)
		{
			XmlElement element = xmlDocument.CreateElement(name);
			parent.AppendChild(element);

			return element;
		}
		
		public void Remove(XmlElement xmlElement)
		{
			xmlRoot.RemoveChild(xmlElement);
		}

		public XmlElement Find(string name)
		{
			string[] childs = name.Split(SPLIT);

			XmlElement xmlElement = xmlRoot;

			foreach (string child in childs)
			{
				xmlElement = xmlElement.SelectSingleNode(child) as XmlElement;
				if (null == xmlElement)
				{
					return null;
				}
			}

			return xmlElement;
		}

		public XmlNodeList FindAll(string name)
		{
			string[] childs = name.Split(SPLIT);

			XmlElement xmlElement = xmlRoot;

			for (int i = 0; i < childs.Length; i++)
			{
				if (i == childs.Length - 1)
				{
					return xmlElement.SelectNodes(childs[i]);
				}
				else
				{
					xmlElement = xmlElement.SelectSingleNode(childs[i]) as XmlElement;
					if (null == xmlElement)
					{
						return null;
					}
				}
			}

			return null;
		}
	}
}