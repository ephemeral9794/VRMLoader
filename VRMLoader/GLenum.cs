﻿namespace VRMLoader {
	public enum GLenum {
		GL_BYTE = 0x1400,
		GL_UNSIGNED_BYTE = 0x1401,
		GL_SHORT = 0x1402,
		GL_UNSIGNED_SHORT = 0x1403,
		GL_INT = 0x1404,
		GL_UNSIGNED_INT = 0x1405,
		GL_FLOAT = 0x1406,
		GL_2_BYTES = 0x1407,
		GL_3_BYTES = 0x1408,
		GL_4_BYTES = 0x1409,
		GL_DOUBLE = 0x140A,

		/* TextureMagFilter */
		GL_NEAREST = 0x2600,
		GL_LINEAR = 0x2601,

		/* TextureMinFilter */
		GL_NEAREST_MIPMAP_NEAREST = 0x2700,
		GL_LINEAR_MIPMAP_NEAREST = 0x2701,
		GL_NEAREST_MIPMAP_LINEAR = 0x2702,
		GL_LINEAR_MIPMAP_LINEAR = 0x2703,

		/* TextureWrapMode */
		GL_CLAMP = 0x2900,
		GL_REPEAT = 0x2901,

		/* BeginMode */
		GL_POINTS = 0x0000,
		GL_LINES = 0x0001,
		GL_LINE_LOOP = 0x0002,
		GL_LINE_STRIP = 0x0003,
		GL_TRIANGLES = 0x0004,
		GL_TRIANGLE_STRIP = 0x0005,
		GL_TRIANGLE_FAN = 0x0006,
		GL_QUADS = 0x0007,
		GL_QUAD_STRIP = 0x0008,
		GL_POLYGON = 0x0009,

		/* BufferTarget */
		GL_ARRAY_BUFFER = 0x8892,
		GL_ELEMENT_ARRAY_BUFFER = 0x8893,

		None = -1,
	}
}