﻿//
// Copyright (c) 2012 Tim Heuer
//
// Licensed under the Microsoft Public License (Ms-PL) (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://opensource.org/licenses/Ms-PL.html
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

// SOME BASE CODE PROVIDED UNDER THIS LICENSE
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
//
// SOME PORTIONS ALSO PROVIDED UNDER THIS LICENSE
// Apache License
// Version 2.0, January 2004
// http://www.apache.org/licenses/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Callisto;

namespace Callisto.OAuth
{
    public static class OAuthTools
    {
        private const string AlphaNumeric = Upper + Lower + Digit;
        private const string Digit = "1234567890";
        private const string Lower = "abcdefghijklmnopqrstuvwxyz";
        private const string Unreserved = AlphaNumeric + "-._~";
        private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static readonly Random _random;
        private static readonly object _randomLock = new object();

        static OAuthTools()
        {
            _random = new Random();
        }

        /// <summary>
        /// All text parameters are UTF-8 encoded (per section 5.1).
        /// </summary>
        /// <seealso cref="http://www.hueniverse.com/hueniverse/2008/10/beginners-gui-1.html"/> 
        private static readonly Encoding _encoding = Encoding.UTF8;

        /// <summary>
        /// Generates a random 16-byte lowercase alphanumeric string. 
        /// </summary>
        /// <seealso cref="http://oauth.net/core/1.0#nonce"/>
        /// <returns></returns>
        public static string GetNonce()
        {
            const string chars = (Lower + Digit);

            var nonce = new char[16];
            lock (_randomLock)
            {
                for (var i = 0; i < nonce.Length; i++)
                {
                    nonce[i] = chars[_random.Next(0, chars.Length)];
                }
            }
            return new string(nonce);
        }

        /// <summary>
        /// Generates a timestamp based on the current elapsed seconds since '01/01/1970 0000 GMT"
        /// </summary>
        /// <seealso cref="http://oauth.net/core/1.0#nonce"/>
        /// <returns></returns>
        public static string GetTimestamp()
        {
            return GetTimestamp(DateTime.UtcNow);
        }

        /// <summary>
        /// Generates a timestamp based on the elapsed seconds of a given time since '01/01/1970 0000 GMT"
        /// </summary>
        /// <seealso cref="http://oauth.net/core/1.0#nonce"/>
        /// <param name="dateTime">A specified point in time.</param>
        /// <returns></returns>
        public static string GetTimestamp(DateTime dateTime)
        {
            var timestamp = dateTime.ToUnixTime();
            return timestamp.ToString();
        }

        /// <summary>
        /// URL encodes a string based on section 5.1 of the OAuth spec.
        /// Namely, percent encoding with [RFC3986], avoiding unreserved characters,
        /// upper-casing hexadecimal characters, and UTF-8 encoding for text value pairs.
        /// </summary>
        /// <param name="value"></param>
        /// <seealso cref="http://oauth.net/core/1.0#encoding_parameters" />
        public static string UrlEncodeRelaxed(string value)
        {
            var escaped = Uri.EscapeDataString(value);

            // LinkedIn users have problems because it requires escaping brackets
            escaped = escaped.Replace("(", "(".PercentEncode())
                             .Replace(")", ")".PercentEncode());

            return escaped;
        }

        /// <summary>
        /// URL encodes a string based on section 5.1 of the OAuth spec.
        /// Namely, percent encoding with [RFC3986], avoiding unreserved characters,
        /// upper-casing hexadecimal characters, and UTF-8 encoding for text value pairs.
        /// </summary>
        /// <param name="value"></param>
        /// <seealso cref="http://oauth.net/core/1.0#encoding_parameters" />
        public static string UrlEncodeStrict(string value)
        {
            // [JD]: We need to escape the apostrophe as well or the signature will fail
            var original = value;
            var ret = original; /*.Where(
                c => !Unreserved.Contains(c) && c != '%').Aggregate(
                    value, (current, c) => current.Replace(
                          c.ToString(), c.ToString().PercentEncode()
                          ));*/

            return ret.Replace("%%", "%25%"); // Revisit to encode actual %'s
        }

        /// <summary>
        /// Sorts a collection of key-value pairs by name, and then value if equal,
        /// concatenating them into a single string. This string should be encoded
        /// prior to, or after normalization is run.
        /// </summary>
        /// <seealso cref="http://oauth.net/core/1.0#rfc.section.9.1.1"/>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string NormalizeRequestParameters(WebParameterCollection parameters)
        {
            var copy = SortParametersExcludingSignature(parameters);
            var concatenated = copy.Concatenate("=", "&");
            return concatenated;
        }

        /// <summary>
        /// Sorts a <see cref="WebParameterCollection"/> by name, and then value if equal.
        /// </summary>
        /// <param name="parameters">A collection of parameters to sort</param>
        /// <returns>A sorted parameter collection</returns>
        public static WebParameterCollection SortParametersExcludingSignature(WebParameterCollection parameters)
        {
            var copy = new WebParameterCollection(parameters);
            var exclusions = copy.Where(n => n.Name.EqualsIgnoreCase("oauth_signature"));

            copy.RemoveAll(exclusions);
            copy.ForEach(p => p.Value = UrlEncodeStrict(p.Value));
            // TODO
            //copy.Sort((x, y) => x.Name.Equals(y.Name) ? x.Value.CompareTo(y.Value) : x.Name.CompareTo(y.Name));
            return copy;
        }

        /// <summary>
        /// Creates a request URL suitable for making OAuth requests.
        /// Resulting URLs must exclude port 80 or port 443 when accompanied by HTTP and HTTPS, respectively.
        /// Resulting URLs must be lower case.
        /// </summary>
        /// <seealso cref="http://oauth.net/core/1.0#rfc.section.9.1.2"/>
        /// <param name="url">The original request URL</param>
        /// <returns></returns>
        public static string ConstructRequestUrl(Uri url)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            var sb = new StringBuilder();

            var requestUrl = "{0}://{1}".FormatWith(url.Scheme, url.Host);
            var qualified = ":{0}".FormatWith(url.Port);
            var basic = url.Scheme == "http" && url.Port == 80;
            var secure = url.Scheme == "https" && url.Port == 443;

            sb.Append(requestUrl);
            sb.Append(!basic && !secure ? qualified : "");
            sb.Append(url.AbsolutePath);

            return sb.ToString(); //.ToLower();
        }

        /// <summary>
        /// Creates a request elements concatentation value to send with a request. 
        /// This is also known as the signature base.
        /// </summary>
        /// <seealso cref="http://oauth.net/core/1.0#rfc.section.9.1.3"/>
        /// <seealso cref="http://oauth.net/core/1.0#sig_base_example"/>
        /// <param name="method">The request's HTTP method type</param>
        /// <param name="url">The request URL</param>
        /// <param name="parameters">The request's parameters</param>
        /// <returns>A signature base string</returns>
        public static string ConcatenateRequestElements(WebMethod method, string url, WebParameterCollection parameters)
        {
            var sb = new StringBuilder();

            // Separating &'s are not URL encoded
            var requestMethod = method.ToUpper().Then("&");
            var requestUrl = UrlEncodeRelaxed(ConstructRequestUrl(url.AsUri())).Then("&");
            var requestParameters = UrlEncodeRelaxed(NormalizeRequestParameters(parameters));

            sb.Append(requestMethod);
            sb.Append(requestUrl);
            sb.Append(requestParameters);

            return sb.ToString();
        }

        /// <summary>
        /// Creates a signature value given a signature base and the consumer secret.
        /// This method is used when the token secret is currently unknown.
        /// </summary>
        /// <seealso cref="http://oauth.net/core/1.0#rfc.section.9.2"/>
        /// <param name="signatureMethod">The hashing method</param>
        /// <param name="signatureBase">The signature base</param>
        /// <param name="consumerSecret">The consumer key</param>
        /// <returns></returns>
        public static string GetSignature(OAuthSignatureMethod signatureMethod,
                                          string signatureBase,
                                          string consumerSecret)
        {
            return GetSignature(signatureMethod, OAuthSignatureTreatment.Escaped, signatureBase, consumerSecret, null);
        }

        /// <summary>
        /// Creates a signature value given a signature base and the consumer secret.
        /// This method is used when the token secret is currently unknown.
        /// </summary>
        /// <seealso cref="http://oauth.net/core/1.0#rfc.section.9.2"/>
        /// <param name="signatureMethod">The hashing method</param>
        /// <param name="signatureTreatment">The treatment to use on a signature value</param>
        /// <param name="signatureBase">The signature base</param>
        /// <param name="consumerSecret">The consumer key</param>
        /// <returns></returns>
        public static string GetSignature(OAuthSignatureMethod signatureMethod,
                                          OAuthSignatureTreatment signatureTreatment,
                                          string signatureBase,
                                          string consumerSecret)
        {
            return GetSignature(signatureMethod, signatureTreatment, signatureBase, consumerSecret, null);
        }

        /// <summary>
        /// Creates a signature value given a signature base and the consumer secret and a known token secret.
        /// </summary>
        /// <seealso cref="http://oauth.net/core/1.0#rfc.section.9.2"/>
        /// <param name="signatureMethod">The hashing method</param>
        /// <param name="signatureBase">The signature base</param>
        /// <param name="consumerSecret">The consumer secret</param>
        /// <param name="tokenSecret">The token secret</param>
        /// <returns></returns>
        public static string GetSignature(OAuthSignatureMethod signatureMethod,
                                          string signatureBase,
                                          string consumerSecret,
                                          string tokenSecret)
        {
            return GetSignature(signatureMethod, OAuthSignatureTreatment.Escaped, consumerSecret, tokenSecret);
        }

        /// <summary>
        /// Creates a signature value given a signature base and the consumer secret and a known token secret.
        /// </summary>
        /// <seealso cref="http://oauth.net/core/1.0#rfc.section.9.2"/>
        /// <param name="signatureMethod">The hashing method</param>
        /// <param name="signatureTreatment">The treatment to use on a signature value</param>
        /// <param name="signatureBase">The signature base</param>
        /// <param name="consumerSecret">The consumer secret</param>
        /// <param name="tokenSecret">The token secret</param>
        /// <returns></returns>
        public static string GetSignature(OAuthSignatureMethod signatureMethod,
                                          OAuthSignatureTreatment signatureTreatment,
                                          string signatureBase,
                                          string consumerSecret,
                                          string tokenSecret)
        {
            if (tokenSecret.IsNullOrBlank())
            {
                tokenSecret = String.Empty;
            }

            consumerSecret = UrlEncodeRelaxed(consumerSecret);
            tokenSecret = UrlEncodeRelaxed(tokenSecret);

            string signature;
            switch (signatureMethod)
            {
                case OAuthSignatureMethod.HmacSha1:
                    {
                        var crypto = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha1);
                        var key = "{0}&{1}".FormatWith(consumerSecret, tokenSecret);
                        signature = signatureBase.HashWith(crypto, key);
                        break;
                    }
                default:
                    throw new NotImplementedException("Only HMAC-SHA1 is currently supported.");
            }

            var result = signatureTreatment == OAuthSignatureTreatment.Escaped
                       ? UrlEncodeRelaxed(signature)
                       : signature;

            return result;
        }
    }

    public enum OAuthSignatureMethod
    {
        HmacSha1,
        PlainText,
        RsaSha1
    }

    public enum OAuthSignatureTreatment
    {
        Escaped,
        Unescaped
    }

    public class WebPairCollection : IList<WebPair>
    {
        private IList<WebPair> _parameters;

        public virtual WebPair this[string name]
        {
            get
            {
                var parameters = this.Where(p => p.Name.Equals(name));

                if (parameters.Count() == 0)
                {
                    return null;
                }

                if (parameters.Count() == 1)
                {
                    return parameters.Single();
                }

                var value = string.Join(",", parameters.Select(p => p.Value).ToArray());
                return new WebPair(name, value);
            }
        }

        public virtual IEnumerable<string> Names
        {
            get { return _parameters.Select(p => p.Name); }
        }

        public virtual IEnumerable<string> Values
        {
            get { return _parameters.Select(p => p.Value); }
        }

        public WebPairCollection(IEnumerable<WebPair> parameters)
        {
            _parameters = new List<WebPair>(parameters);
        }

        public WebPairCollection(NameValueCollection collection)
            : this()
        {
            AddCollection(collection);
        }

        public virtual void AddRange(NameValueCollection collection)
        {
            AddCollection(collection);
        }

        private void AddCollection(NameValueCollection collection)
        {
            var parameters = collection.AllKeys.Select(key => new WebPair(key, collection[key]));
            foreach (var parameter in parameters)
            {
                _parameters.Add(parameter);
            }
        }

        public WebPairCollection(IDictionary<string, string> collection)
            : this()
        {
            AddCollection(collection);
        }

        public void AddCollection(IDictionary<string, string> collection)
        {
            foreach (var parameter in collection.Keys.Select(key => new WebPair(key, collection[key])))
            {
                _parameters.Add(parameter);
            }
        }

        public WebPairCollection()
        {
            _parameters = new List<WebPair>(0);
        }

        public WebPairCollection(int capacity)
        {
            _parameters = new List<WebPair>(capacity);
        }

        private void AddCollection(IEnumerable<WebPair> collection)
        {
            foreach (var pair in collection.Select(parameter => new WebPair(parameter.Name, parameter.Value)))
            {
                _parameters.Add(pair);
            }
        }

        public virtual void AddRange(WebPairCollection collection)
        {
            AddCollection(collection);
        }

        public virtual void AddRange(IEnumerable<WebPair> collection)
        {
            AddCollection(collection);
        }

        //public virtual void Sort(Comparison<WebPair> comparison)
        //{
        //    var sorted = new List<WebPair>(_parameters);
        //    sorted.Sort(comparison);
        //    _parameters = sorted;
        //}

        public virtual bool RemoveAll(IEnumerable<WebPair> parameters)
        {
            var array = parameters.ToArray();
            var success = array.Aggregate(true, (current, parameter) => current & _parameters.Remove(parameter));
            return success && array.Length > 0;
        }

        public virtual void Add(string name, string value)
        {
            var pair = new WebPair(name, value);
            _parameters.Add(pair);
        }

        #region IList<WebParameter> Members

        public virtual IEnumerator<WebPair> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual void Add(WebPair parameter)
        {

            _parameters.Add(parameter);
        }

        public virtual void Clear()
        {
            _parameters.Clear();
        }

        public virtual bool Contains(WebPair parameter)
        {
            return _parameters.Contains(parameter);
        }

        public virtual void CopyTo(WebPair[] parameters, int arrayIndex)
        {
            _parameters.CopyTo(parameters, arrayIndex);
        }

        public virtual bool Remove(WebPair parameter)
        {
            return _parameters.Remove(parameter);
        }

        public virtual int Count
        {
            get { return _parameters.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return _parameters.IsReadOnly; }
        }

        public virtual int IndexOf(WebPair parameter)
        {
            return _parameters.IndexOf(parameter);
        }

        public virtual void Insert(int index, WebPair parameter)
        {
            _parameters.Insert(index, parameter);
        }

        public virtual void RemoveAt(int index)
        {
            _parameters.RemoveAt(index);
        }

        public virtual WebPair this[int index]
        {
            get { return _parameters[index]; }
            set { _parameters[index] = value; }
        }

        #endregion
    }

    public class WebPair
    {
        public WebPair(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Value { get; set; }
        public string Name { get; private set; }
    }

    public class WebParameter : WebPair
    {
        public WebParameter(string name, string value)
            : base(name, value)
        {

        }
    }

    public class WebParameterCollection : WebPairCollection
    {
        public WebParameterCollection(IEnumerable<WebPair> parameters)
            : base(parameters)
        {

        }

        public WebParameterCollection(NameValueCollection collection)
            : base(collection)
        {
        }

        public WebParameterCollection()
        {
        }

        public WebParameterCollection(int capacity)
            : base(capacity)
        {
        }

        public WebParameterCollection(IDictionary<string, string> collection)
            : base(collection)
        {

        }

        public override void Add(string name, string value)
        {
            var parameter = new WebParameter(name, value);
            base.Add(parameter);
        }
    }

    public class NameValueCollection : List<KeyValuePair<string, string>>
    {
        public new string this[int index]
        {
            get
            {
                return base[index].Value;
            }
        }

        public string this[string name]
        {
            get
            {
                return this.SingleOrDefault(kv => kv.Key.Equals(name)).Value;
            }
        }

        public NameValueCollection()
        {

        }

        public NameValueCollection(int capacity)
            : base(capacity)
        {

        }

        public void Add(string name, string value)
        {
            Add(new KeyValuePair<string, string>(name, value));
        }

        public IEnumerable<string> AllKeys
        {
            get
            {
                return this.Select(pair => pair.Key);
            }
        }
    }

    public enum WebMethod
    {
        Get,
        Post,
        Delete,
        Put,
        Head,
        Options
    }
}