// Copyright 2020 J. KIEFFER - All Rights Reserved.
using System;
using System.Collections.Generic;

namespace vzp {
	public class StaticIndexer<T> {
		//=============================================================================================
		struct Index {
			public int index;
			public T value;
		}

		//=============================================================================================
		struct IndexComparer : IComparer<Index> {
			public int Compare( Index x, Index y ) {
				return x.index.CompareTo( y.index );
			}
		}

		//=============================================================================================
		Index[] m_indices;
		Func< T, int > m_indexer;
		IndexComparer m_comparer;

		//=============================================================================================
		public T this[ int _index ] {
			get { return m_indices[ _index ].value; }
		}

		//=============================================================================================
		public int Length {
			get { return m_indices.Length; }
		}

		//=============================================================================================
		public StaticIndexer( IEnumerable<T> _collection, Func<T, int> _indexer ) {
			List<Index> collectionList = new List<Index>();
			foreach ( T element in _collection ) {
				collectionList.Add( new Index() { index = _indexer( element ), value = element } );
			}
			m_indices = collectionList.ToArray();
			m_indexer = _indexer;
			m_comparer = new IndexComparer();

			Array.Sort( m_indices, m_comparer );
		}

		//=============================================================================================
		public int GetIndex( T _value ) {
			Index searchElement = new Index() {
				index = m_indexer( _value ),
				value = default( T )
			};
			return Array.BinarySearch( m_indices, searchElement, m_comparer );
		}
	}
}
