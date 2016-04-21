using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RedSys.RFC.Core.Helper
{
	public class QueryString
	{
		private String m_Url;
		private IDictionary<String, String> m_InternalStorage;

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		protected QueryString()
		{
			m_Url = String.Empty;
			m_InternalStorage = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public QueryString(String url)
			: this()
		{
			ParseRawUrl(url);
		}
		#endregion

		#region Public properties

		public IDictionary<String, String> AllParameters
		{
			get { return m_InternalStorage; }
		}

		/// <summary>
		/// Access an parameter value by the parameter name.
		/// </summary>
		public String this[String parameterName]
		{
			get
			{
				String result = String.Empty;
				if (m_InternalStorage.ContainsKey(parameterName))
				{
					result = HttpUtility.UrlDecode(m_InternalStorage[parameterName]);
				}
				return result;
			}
			set
			{
				SetParameter(parameterName, value);
			}
		}

		/// <summary>
		/// The URL that comes before the actual name-value pair parameters.
		/// </summary>
		public String Url
		{
			get
			{
				return m_Url;
			}
			set
			{
				m_Url = value;
			}
		}

		/// <summary>
		/// For supporting "return mode" where user may complets operations with current page 
		/// and automatically returns to the previous page
		/// Important: it's execute ForceUrlToBeReloaded for return value;
		/// </summary>
		public String SourceUrl
		{
			get
			{
				return this["Source"];
			}
			set
			{
				this["Source"] = value;
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Get the complete string including the Url and
		/// all current parameters.
		/// </summary>
		public override String ToString()
		{
			String result = Url + BuildParamsUrlPart();
			return result;
		}

		public Boolean GetParamAsBoolean(String paramName, Boolean defaultValue)
		{
			Boolean result = defaultValue;
			if (this[paramName].Length > 0)
			{
				if (
					this[paramName].ToLower() == Boolean.TrueString.ToLower()
					|| this[paramName].ToLower() == Boolean.FalseString.ToLower()
					)
				{
					result = Convert.ToBoolean(this[paramName]);
				}
			}
			return result;
		}

		public Int32 GetParamAsInteger(String paramName, Int32 defaultValue)
		{
			Int32 result = defaultValue;
			if (this[paramName].Length > 0)
			{
				try
				{
					result = Int32.Parse(this[paramName]);
				}
				catch (Exception)
				{

				}
			}
			return result;
		}

		public void ReInit(String url)
		{
			m_Url = String.Empty;
			m_InternalStorage.Clear();
			ParseRawUrl(url);
		}

		public static String ForceUrlToBeReloaded(String sUrl)
		{
			QueryString oQS = new QueryString(sUrl);
			oQS["TM"] = new Random().NextDouble().ToString();
			return oQS.ToString();
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Set or replace a single parameter.
		/// </summary>
		/// <param name="name">The name of the parameter to set.</param>
		/// <param name="val">The value of the parameter to set.</param>
		private void SetParameter(String name, String val)
		{
			m_InternalStorage[name] = HttpUtility.UrlEncode(val);
		}

		/// <summary>
		/// Parse a query string and insert the found parameters
		/// into the collection of this class.
		/// </summary>
		private void ParseRawUrl(String url)
		{
			if (url != null)
			{
				//if (url.Length > 2048)
				//{
				//    throw new NotSupportedException("Too long URL: length=" + url.Length);
				//}

				m_InternalStorage.Clear();

				// store the part before, too.
				int qPos = url.IndexOf("?");
				if (qPos >= 0)
				{
					Url = url.Substring(0, qPos - 0);
					url = url.Substring(qPos + 1);
				}
				else
				{
					Url = url;
				}

				if (url.Length > 0 && url.Substring(0, 1) == "?")
				{
					url = url.Substring(1);
				}

				// break the values.
				String[] pairs = url.Split('&');
				foreach (String pair in pairs)
				{
					String a = String.Empty;
					String b = String.Empty;

					String[] singular = pair.Split('=');

					int j = 0;
					foreach (String one in singular)
					{
						if (j == 0)
						{
							a = one;
						}
						else
						{
							b = one;
						}

						j++;
					}

					// store.
					SetParameter(a, HttpUtility.UrlDecode(b));
				}
			}
		}

		private String BuildParamsUrlPart()
		{
			String result = "?";

			foreach (String name in m_InternalStorage.Keys)
			{
				String val = m_InternalStorage[name];

				if (!string.IsNullOrEmpty(val))
					result += name + "=" + val + "&";
			}

			//return result;
			return result.TrimEnd('?', '&');
		}
	}
	#endregion
}
