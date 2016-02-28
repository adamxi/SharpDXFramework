using Newtonsoft.Json;

namespace DXFramework.Util
{
	public static class SerializationHelper
	{
		private static JsonSerializerSettings serializerSettings;

		static SerializationHelper()
		{
			serializerSettings = new JsonSerializerSettings()
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				TypeNameHandling = TypeNameHandling.Auto
			};
#if DEBUG
			serializerSettings.Formatting = Formatting.Indented;
#endif
		}

		/// <summary>
		/// Serialize the object to JSON
		/// </summary>
		public static string SerializeToJson(this object obj)
		{
			return JsonConvert.SerializeObject(obj, serializerSettings);
		}

		/// <summary>
		/// Deserializes a JSON string into an object.
		/// </summary>
		/// <typeparam name="T">Object type.</typeparam>
		/// <param name="json">JSON string to deserialize.</param>
		public static T DeserializeFromJson<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json, serializerSettings);
		}

		/// <summary>
		/// Deep clones the object, returning a new identical instance.
		/// </summary>
		public static T DeepClone<T>(this T obj)
		{
			var json = obj.SerializeToJson();
			return DeserializeFromJson<T>(json);
		}
	}
}