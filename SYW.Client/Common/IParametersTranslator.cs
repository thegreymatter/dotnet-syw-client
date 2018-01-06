using System.Collections.Generic;

namespace Syw.Client.Common
{
	internal interface IParametersTranslator
	{
		string ToJson(KeyValuePair<string, object> parameter);
	}
}