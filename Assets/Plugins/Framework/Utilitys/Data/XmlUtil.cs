using System.IO;
using System.Xml;

namespace Framework
{
	public sealed class XmlUtil
	{
		public const char SPLIT = '/';
		public const string ELEMENT_ROOT = "root";

		private XmlDocument m_XmlDoc = new XmlDocument();

		public XmlElement root { get { return m_XmlDoc.DocumentElement; } }

		public XmlUtil(string xml)
		{
			Load(xml);
		}

		public void Load(string xml)
		{
			m_XmlDoc.LoadXml(xml);
		}

		public void Save(string path)
		{
			m_XmlDoc.Save(path);
		}

		public XmlElement Add(string name)
		{
			return Add(root, name);
		}

		public XmlElement Add(XmlElement parent, string name)
		{
			XmlElement element = m_XmlDoc.CreateElement(name);
			parent.AppendChild(element);

			return element;
		}

		public XmlElement Get(string name)
		{
			string[] childs = name.Split(SPLIT);

			XmlElement xmlElement = root;

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

		public XmlNodeList GetAll(string name)
		{
			string[] childs = name.Split(SPLIT);

			XmlElement xmlElement = root;

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

		public void Remove(XmlElement xmlElement)
		{
			root.RemoveChild(xmlElement);
		}
	}
}