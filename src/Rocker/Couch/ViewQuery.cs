﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocker.Couch
{
    public class ViewQuery
    {
        private List<string> _keys;
        private string _endKey;
        private string _name;
        private string _view;
        private int? _take;
        private int? _skip;
        private bool _include_docs;
        private bool _descending;
        private string _urlpattern = "_design/{0}/_view/{1}";
        private bool _multikey = false;
        public bool IncludingDocs { get { return _include_docs; } }
        public ViewQuery(string name, string view)
        {
            _name = name;
            _view = view;
            _descending = false;
            _include_docs = false;
        }

        public ViewQuery Key(string key)
        {
            if (_keys == null)
                _keys = new List<string>();
            else
                _multikey = true;

            _keys.Add(key);
            
            return this;
        }

        public ViewQuery Keys(string[] keys)
        {
            if (_keys == null)
                _keys = new List<string>();

            _keys.AddRange(keys);
            _multikey = true;

            return this;
        }

        public ViewQuery EndKey(string key)
        {
            _endKey = key;
            return this;
        }

        public ViewQuery Take(int count)
        {
            _take = count;
            return this;
        }

        public ViewQuery Skip(int count)
        {
            _skip = count;
            return this;
        }

        public ViewQuery IncludeDocs()
        {
            _include_docs = true;
            return this;
        }

        public ViewQuery OrderDescending(bool desc)
        {
            _descending = desc;
            return this;
        }
        public ViewQuery OrderDescending()
        {
            return OrderDescending(true);
        }

        public string Method
        {
            get;
            private set;
        }

        public object RequestData
        {
            get;
            private set;
        }
        
        public ViewQuery SetUrlPattern(string pattern)
        {
            _urlpattern = pattern;
            return this;
        }
        public string GenerateQuery()
        {
            string q = "";

                q = string.Format(_urlpattern, _name, _view);

                Method = "GET";

            if (_keys != null)
            {
                if (!_multikey)
                {

                    RequestData = null;
                    if (string.IsNullOrEmpty(_endKey))
                    {
                        q = AddQueryStirng(q, "key", _keys.First());
                    }
                    else
                    {
                        if (_descending)
                        {
                            q = AddQueryStirng(q, "endkey", _keys.First());

                            q = AddQueryStirng(q, "startkey", _endKey);
                        }
                        else
                        {
                            q = AddQueryStirng(q, "startkey", _keys.First());

                            q = AddQueryStirng(q, "endkey", _endKey);
                        }
                    }
                }
                else
                {
                    RequestData = (new { keys = _keys });

                    Method = "POST";
                }
            }

            if (_take.HasValue)
                q = AddQueryStirng(q, "limit", _take.Value);

            if (_skip.HasValue)
                q = AddQueryStirng(q, "skip", _skip.Value);
            if (_descending)
                q = AddQueryStirng(q, "descending", "true");
            if (_include_docs)
                q = AddQueryStirng(q, "include_docs", "true");
            return q;
        }

        public string AddQueryStirng(string qs, string name, object value)
        {
            if (qs.Contains("?"))
                return string.Concat(qs, "&", name, "=", value);
            else
                return string.Concat(qs, "?", name, "=", value);
        }
    }
}
