using System.Collections.Generic;

namespace CorruptedSmileStudio.INI
{
    /// <summary>
    /// Contains the methods for reading INI entries.
    /// Reads string, integer, float and bool.
    /// Can also change and remove entries.
    /// </summary>
    public class INIContent
    {
        /// <summary>
        /// The list of elements.
        /// </summary>
        public Dictionary<string, string> elements = new Dictionary<string, string>();

        /// <summary>
        /// Whether the file has loaded or not.
        /// </summary>
        private bool loaded = false;

        /// <summary>
        /// Returns true if the INI file has been loaded
        /// </summary>
        public bool isLoaded
        {
            get
            {
                return loaded;
            }
        }

        /// <summary>
        /// Clears the loaded elements settings.
        /// Doesn't affect the actual file, unless Save is called.
        /// Sets isLoaded to false.
        /// </summary>
        public void Clear()
        {
            elements.Clear();
            loaded = false;
        }

        #region Get
        private string Get(string ID, string section)
        {
            string value;

            if (!elements.TryGetValue(string.Format("{0}.{1}", section, ID), out value))
                value = null;

            return value;
        }
        /// <summary>
        /// Returns a string element
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <returns>The value associated with the identity.</returns>
        public string GetString(string ID)
        {
            return GetString(ID, "", "");
        }
        /// <summary>
        /// Returns a string element
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <returns>The value associated with the identity.</returns>
        public string GetString(string ID, string section)
        {
            return GetString(ID, section, "");
        }
        /// <summary>
        /// Returns a string element
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <param name="defaultValue">If no value is found, this is the default value to return.</param>
        /// <returns>The value associated with the identity.</returns>
        public string GetString(string ID, string section, string defaultValue)
        {
            string value = Get(ID, section);

            if (value == null)
                value = defaultValue;

            return value;
        }
        /// <summary>
        /// Returns the element as a Float.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <returns>The value associated with the identity.</returns>
        public float GetFloat(string ID)
        {
            return GetFloat(ID, "", 0.0f);
        }
        /// <summary>
        /// Returns the element as a Float.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <returns>The value associated with the identity.</returns>
        public float GetFloat(string ID, string section)
        {
            return GetFloat(ID, section, 0.0f);
        }
        /// <summary>
        /// Returns the element as a Float.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <param name="defaultValue">If no value is found or if parsing fails, this is the default value to return.</param>
        /// <returns>The value associated with the identity.</returns>
        public float GetFloat(string ID, string section, float defaultValue)
        {
            string value;
            float returnValue;

            if (!elements.TryGetValue(string.Format("{0}.{1}", section, ID), out value))
                returnValue = defaultValue;
            else
            {
                try
                {
                    float.TryParse(value, out returnValue);
                }
                catch (System.Exception)
                {
                    returnValue = defaultValue;
                }
            }

            return returnValue;
        }
        /// <summary>
        /// Returns the element as a Int.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <returns>The value associated with the identity.</returns>
        public int GetInt(string ID)
        {
            return GetInt(ID, "", 0);
        }
        /// <summary>
        /// Returns the element as a Int.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <returns>The value associated with the identity.</returns>
        public int GetInt(string ID, string section)
        {
            return GetInt(ID, section, 0);
        }
        /// <summary>
        /// Returns the element as a Int.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <param name="defaultValue">If no value is found or parsing fails, this is the default value to return.</param>
        /// <returns>The value associated with the identity.</returns>
        public int GetInt(string ID, string section, int defaultValue)
        {
            string value;
            int returnValue;

            if (!elements.TryGetValue(string.Format("{0}.{1}", section, ID), out value))
                returnValue = defaultValue;
            else
            {
                try
                {
                    int.TryParse(value, out returnValue);
                }
                catch (System.Exception)
                {
                    returnValue = defaultValue;
                }
            }

            return returnValue;
        }
        /// <summary>
        /// Returns the element as a Bool.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <returns>The value associated with the identity.</returns>
        public bool GetBool(string ID)
        {
            return GetBool(ID, "", true);
        }
        /// <summary>
        /// Returns the element as a Bool.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <returns>The value associated with the identity.</returns>
        public bool GetBool(string ID, string section)
        {
            return GetBool(ID, section, true);
        }
        /// <summary>
        /// Returns the element as a Bool.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <param name="defaultValue">If no value is found or if parsing fails, this is the default value to return.</param>
        /// <returns>The value associated with the identity.</returns>
        public bool GetBool(string ID, string section, bool defaultValue)
        {
            string value;
            bool returnValue;

            if (!elements.TryGetValue(string.Format("{0}.{1}", section, ID), out value))
                returnValue = defaultValue;
            else
            {
                try
                {
                    bool.TryParse(value, out returnValue);
                }
                catch
                {
                    returnValue = defaultValue;
                }
            }

            return returnValue;
        }
        #endregion
        #region Change
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        public void Change(string ID, string value)
        {
            Change(ID, value, "");
        }
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        /// <param name="section">The section the identity falls under.</param>
        public void Change(string ID, string value, string section)
        {
            if (!elements.ContainsKey(string.Format("{0}.{1}", section, ID)))
            {
                elements.Add(string.Format("{0}.{1}", section, ID), value);
            }
            else
            {
                elements[string.Format("{0}.{1}", section, ID)] = value;
            }
        }
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        public void Change(string ID, int value)
        {
            Change(ID, value, "");
        }
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        /// <param name="section">The section the identity falls under.</param>
        public void Change(string ID, int value, string section)
        {
            if (!elements.ContainsKey(string.Format("{0}.{1}", section, ID)))
            {
                // Locks so that only this thread can access the elements variable.
                elements.Add(string.Format("{0}.{1}", section, ID), value.ToString());
            }
            else
            {
                elements[string.Format("{0}.{1}", section, ID)] = value.ToString();
            }
        }
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        public void Change(string ID, bool value)
        {
            Change(ID, value, "");
        }
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        /// <param name="section">The section the identity falls under.</param>
        public void Change(string ID, bool value, string section)
        {
            if (!elements.ContainsKey(string.Format("{0}.{1}", section, ID)))
            {
                elements.Add(string.Format("{0}.{1}", section, ID), value.ToString());
            }
            else
            {
                elements[string.Format("{0}.{1}", section, ID)] = value.ToString();
            }
        }
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        public void Change(string ID, float value)
        {
            Change(ID, value, "");
        }
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        /// <param name="section">The section the identity falls under.</param>
        public void Change(string ID, float value, string section)
        {
            if (!elements.ContainsKey(string.Format("{0}.{1}", section, ID)))
            {
                elements.Add(string.Format("{0}.{1}", section, ID), value.ToString());
            }
            else
            {
                elements[string.Format("{0}.{1}", section, ID)] = value.ToString();
            }
        }
        #endregion
        #region Remove
        /// <summary>
        /// Remove a specific element from elements file.
        /// </summary>
        /// <param name="ID">The identity to remove</param>
        /// <returns>True if found and removed or false</returns>
        public bool Remove(string ID)
        {
            return Remove(ID, "");
        }
        /// <summary>
        /// Remove a specific element from elements file.
        /// </summary>
        /// <param name="ID">The identity to remove</param>
        /// <param name="section">The section the identity is in.</param>
        /// <returns>True if found and removed or false</returns>
        public bool Remove(string ID, string section)
        {
            return elements.Remove(string.Format("{0}.{1}", section, ID));
        }
        #endregion
        /// <summary>
        /// Returns a INIUnity object of the INIContent object.
        /// </summary>
        /// <returns>An INIUnity object.</returns>
        public INIUnity ToINIUnity()
        {
            INIUnity content = new INIUnity();
            content.elements = elements;
            content.loaded = loaded;
            return content;
        }
    }
}