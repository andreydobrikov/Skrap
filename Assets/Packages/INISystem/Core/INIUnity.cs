using System.Collections.Generic;
using UnityEngine;

namespace CorruptedSmileStudio.INI
{
    /// <summary>
    /// A Unity specific subclass of INIContent.
    /// This class handles various Unity specific types (Vector2, Vector3, Color and Quaternion).
    /// </summary>
    public class INIUnity : INIContent
    {
        #region Get
        /// <summary>
        /// Returns the element as a Color.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <returns>The value associated with the identity.</returns>
        public Color GetColor(string ID)
        {
            return GetColor(ID, "", Color.white);
        }
        /// <summary>
        /// Returns the element as a Color.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <returns>The value associated with the identity.</returns>
        public Color GetColor(string ID, string section)
        {
            return GetColor(ID, section, Color.white);
        }
        /// <summary>
        /// Returns the element as a Color.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <param name="defaultValue">If no value is found, this is the default value to return.</param>
        /// <returns>The value associated with the identity.</returns>
        public Color GetColor(string ID, string section, Color defaultValue)
        {
            string value = base.GetString(ID, section, string.Format("{0},{1},{2},{3}", defaultValue.r, defaultValue.g, defaultValue.b, defaultValue.a));
            Color returnValue;

            returnValue = new Color();
            returnValue.r = float.Parse(value.Substring(0, value.IndexOf(',')));
            value = value.Remove(0, value.IndexOf(',') + 1);
            returnValue.g = float.Parse(value.Substring(0, value.IndexOf(',')));
            value = value.Remove(0, value.IndexOf(',') + 1);
            returnValue.b = float.Parse(value.Substring(0, value.IndexOf(',')));
            value = value.Remove(0, value.IndexOf(',') + 1);
            returnValue.a = float.Parse(value);

            return returnValue;
        }
        /// <summary>
        /// Returns the element as a Vector2.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <returns>The value associated with the identity.</returns>
        public Vector2 GetVector2(string ID)
        {
            return GetVector2(ID, "", Vector2.zero);
        }
        /// <summary>
        /// Returns the element as a Vector2.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <returns>The value associated with the identity.</returns>
        public Vector2 GetVector2(string ID, string section)
        {
            return GetVector2(ID, section, Vector2.zero);
        }
        /// <summary>
        /// Returns the element as a Vector2.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <param name="defaultValue">If no value is found, this is the default value to return.</param>
        /// <returns>The value associated with the identity.</returns>
        public Vector2 GetVector2(string ID, string section, Vector2 defaultValue)
        {
            string value = base.GetString(ID, section, string.Format("{0},{1}", defaultValue.x, defaultValue.y));
            Vector2 returnValue;

            returnValue = new Vector2();
            returnValue.x = int.Parse(value.Substring(0, value.IndexOf(',')));
            returnValue.y = int.Parse(value.Substring(value.IndexOf(',') + 1));

            return returnValue;
        }
        /// <summary>
        /// Returns the element as a Vector3.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <returns>The value associated with the identity.</returns>
        public Vector3 GetVector3(string ID)
        {
            return GetVector3(ID, "", Vector3.zero);
        }
        /// <summary>
        /// Returns the element as a Vector3.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <returns>The value associated with the identity.</returns>
        public Vector3 GetVector3(string ID, string section)
        {
            return GetVector3(ID, section, Vector3.zero);
        }
        /// <summary>
        /// Returns the element as a Vector3.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <param name="defaultValue">If no value is found, this is the default value to return.</param>
        /// <returns>The value associated with the identity.</returns>
        public Vector3 GetVector3(string ID, string section, Vector3 defaultValue)
        {
            string value = base.GetString(ID, section, string.Format("{0},{1},{2}", defaultValue.x, defaultValue.y, defaultValue.z));
            Vector3 returnValue;

            returnValue = new Vector3();
            returnValue.x = float.Parse(value.Substring(0, value.IndexOf(',')));
            value = value.Remove(0, value.IndexOf(',') + 1);
            returnValue.y = float.Parse(value.Substring(0, value.IndexOf(',')));
            value = value.Remove(0, value.IndexOf(',') + 1);
            returnValue.z = float.Parse(value);

            return returnValue;
        }
        /// <summary>
        /// Returns the element as a Quaternion.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <returns>The value associated with the identity.</returns>
        public Quaternion GetQuaternion(string ID)
        {
            return GetQuaternion(ID, "", Quaternion.identity);
        }
        /// <summary>
        /// Returns the element as a Quaternion.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <returns>The value associated with the identity.</returns>
        public Quaternion GetQuaternion(string ID, string section)
        {
            return GetQuaternion(ID, section, Quaternion.identity);
        }
        /// <summary>
        /// Returns the element as a Quaternion.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="section">The section the identity is in.</param>
        /// <param name="defaultValue">If no value is found, this is the default value to return.</param>
        /// <returns>The value associated with the identity.</returns>
        public Quaternion GetQuaternion(string ID, string section, Quaternion defaultValue)
        {
            string value = base.GetString(ID, section, string.Format("{0},{1},{2},{3}", defaultValue.x, defaultValue.y, defaultValue.z, defaultValue.w));
            Quaternion returnValue;

            returnValue = new Quaternion();
            returnValue.x = float.Parse(value.Substring(0, value.IndexOf(',')));
            value = value.Remove(0, value.IndexOf(',') + 1);
            returnValue.y = float.Parse(value.Substring(0, value.IndexOf(',')));
            value = value.Remove(0, value.IndexOf(',') + 1);
            returnValue.z = float.Parse(value.Substring(0, value.IndexOf(',')));
            value = value.Remove(0, value.IndexOf(',') + 1);
            returnValue.w = float.Parse(value);

            return returnValue;
        }
        #endregion
        #region Change
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        public void Change(string ID, Color value)
        {
            Change(ID, value, "");
        }
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        /// <param name="section">The section the identity falls under.</param>
        public void Change(string ID, Color value, string section)
        {
            base.Change(ID, string.Format("{0},{1},{2},{3}", value.r, value.g, value.b, value.a), section);
        }
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        public void Change(string ID, Vector2 value)
        {
            Change(ID, value, "");
        }
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        /// <param name="section">The section the identity falls under.</param>
        public void Change(string ID, Vector2 value, string section)
        {
            base.Change(ID, string.Format("{0},{1}", value.x, value.y), section);
        }
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        public void Change(string ID, Vector3 value)
        {
            Change(ID, value, "");
        }
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        /// <param name="section">The section the identity falls under.</param>
        public void Change(string ID, Vector3 value, string section)
        {
            base.Change(ID, string.Format("{0},{1},{2}", value.x, value.y, value.z), section);
        }
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>        
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        public void Change(string ID, Quaternion value)
        {
            Change(ID, value, "");
        }
        /// <summary>
        /// Changes the value of an element or adds it to the list.
        /// </summary>
        /// <param name="ID">The identity</param>
        /// <param name="value">The value to be set</param>
        /// <param name="section">The section the identity falls under.</param>
        public void Change(string ID, Quaternion value, string section)
        {
            base.Change(ID, string.Format("{0},{1},{2},{3}", value.x, value.y, value.z, value.w), section);
        }
        #endregion
        /// <summary>
        /// Returns the INIUnity object as a INIContent object.
        /// </summary>
        /// <returns>An INIContent object.</returns>
        public INIContent ToINIContent()
        {
            INIContent content = new INIContent();
            content.elements = elements;
            return content;
        }
    }
}