// Copyright 2020 J. KIEFFER - All Rights Reserved.
using System.IO;

namespace vzp {
	public class MaskedStream : Stream {
		//=============================================================================================
		Stream m_internal = null;
		long m_seed = 0;
		long m_randX = 0;
		const long kRandA = 16807;
		const long kRandM = 2147483647;
		static bool s_debugging = true;

		//=============================================================================================
		static byte[] s_lookup = new byte[] {
			0x0f, 0x17, 0x1b, 0x1d, 0x1e, 0x27, 0x2b,
			0x2d, 0x2e, 0x33, 0x35, 0x36, 0x39, 0x3a,
			0x3c, 0x47, 0x4b, 0x4d, 0x4e, 0x53, 0x55,
			0x56, 0x59, 0x5a, 0x5c, 0x63, 0x65, 0x66,
			0x69, 0x6a, 0x6c, 0x71, 0x72, 0x74, 0x78,
			0x87, 0x8b, 0x8d, 0x8e, 0x93, 0x95, 0x96,
			0x99, 0x9a, 0x9c, 0xa3, 0xa5, 0xa6, 0xa9,
			0xaa, 0xac, 0xb1, 0xb2, 0xb4, 0xb8, 0xc3,
			0xc5, 0xc6, 0xc9, 0xca, 0xcc, 0xd1, 0xd2,
			0xd4, 0xd8, 0xe1, 0xe2, 0xe4, 0xe8, 0xf0
		};

		//=============================================================================================
		public MaskedStream( Stream _internal, long _seed ) {
			m_internal = _internal;
			m_seed = _seed;
		}

		public override bool CanRead {
			get { return m_internal.CanRead; }
		}

		public override bool CanSeek {
			get { return m_internal.CanSeek; }
		}

		public override bool CanWrite {
			get { return m_internal.CanWrite; }
		}

		public override long Length {
			get { return m_internal.Length; }
		}

		public override long Position {
			get { return m_internal.Position; }
			set { m_internal.Position = value; }
		}

		public override void Flush() {
			m_internal.Flush();
		}

		public override int Read( byte[] buffer, int offset, int count ) {
			if ( s_debugging ) {
				return m_internal.Read( buffer, offset, count );
			} else {
				m_randX = Position ^ m_seed;
				int result = m_internal.Read( buffer, offset, count );
				for ( int i = 0; i < result; ++i ) {
					buffer[ i ] ^= s_lookup[ Rand() % s_lookup.Length ];
				}
				return result;
			}
		}

		public override long Seek( long offset, SeekOrigin origin ) {
			return m_internal.Seek( offset, origin );
		}

		public override void SetLength( long value ) {
			m_internal.SetLength( value );
		}

		public override void Write( byte[] buffer, int offset, int count ) {
			if ( s_debugging ) {
				m_internal.Write( buffer, offset, count );
			} else {
				long position = Position;
				m_randX = position ^ m_seed;
				for ( int i = 0; i < count; ++i ) {
					buffer[ i ] ^= s_lookup[ Rand() % s_lookup.Length ];
				}
				m_internal.Write( buffer, offset, count );
				m_randX = position ^ m_seed;
				for ( int i = 0; i < count; ++i ) {
					buffer[ i ] ^= s_lookup[ Rand() % s_lookup.Length ];
				}
			}
		}

		long Rand() {
			long r = m_randX;
			m_randX = ( kRandA * m_randX ) % kRandM;
			return r;
		}
	}
}
