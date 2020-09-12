using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace ManiacEditor.Classes.Rendering
{
	#region Original C++ Copyright

	//////////////////////////////////////////////////////////////////////////////
	//
	// Selba Ward (https://github.com/Hapaxia/SelbaWard)
	// --
	//
	// Line
	//
	// Copyright(c) 2015-2020 M.J.Silk
	//
	// This software is provided 'as-is', without any express or implied
	// warranty. In no event will the authors be held liable for any damages
	// arising from the use of this software.
	//
	// Permission is granted to anyone to use this software for any purpose,
	// including commercial applications, and to alter it and redistribute it
	// freely, subject to the following restrictions :
	//
	// 1. The origin of this software must not be misrepresented; you must not
	// claim that you wrote the original software.If you use this software
	// in a product, an acknowledgment in the product documentation would be
	// appreciated but is not required.
	//
	// 2. Altered source versions must be plainly marked as such, and must not be
	// misrepresented as being the original software.
	//
	// 3. This notice may not be removed or altered from any source distribution.
	//
	// M.J.Silk
	// MJSilk2@gmail.com
	//
	//////////////////////////////////////////////////////////////////////////////

	#endregion

	public class SelbaWardLine : SFML.Graphics.Transformable, SFML.Graphics.Drawable
	{

		public enum PointIndex
		{
			Start = 0,
			End = 1
		};

		private const float thicknessEpsilon = 0.001f;
		private const float pi = 3.141592653f;

		private VertexArray m_vertices { get; set; } = new VertexArray(PrimitiveType.Lines, 2);
		private VertexArray m_quad { get; set; } = new VertexArray(PrimitiveType.Quads, 4);
		private float m_thickness { get; set; } = 0f;
		private Texture m_texture { get; set; }
		private FloatRect m_textureRect { get; set; }

		public Transform GetTransform()
		{
			return Transform;
		}

		public SelbaWardLine(Vector2f startPosition, Vector2f endPosition)
		{
			SetThickness(0);
			SetColor(SFML.Graphics.Color.White);
			SetPoints(startPosition, endPosition);
		}

		public SelbaWardLine(Vector2f startPosition, Vector2f endPosition, Color color)
		{
			SetThickness(0);
			SetColor(color);
			SetPoints(startPosition, endPosition);
		}
		public SelbaWardLine(Vector2f startPosition, Vector2f endPosition, float thickness)
		{
			SetThickness(thickness);
			SetColor(SFML.Graphics.Color.White);
			SetPoints(startPosition, endPosition);
		}

		public SelbaWardLine(Vector2f startPosition, Vector2f endPosition, Color color, float thickness)
		{
			SetThickness(thickness);
			SetColor(color);
			SetPoints(startPosition, endPosition);
		}

		public void SetPoint(uint index, Vector2f position)
		{
			if (index > 1)
				return;

			ModifyVertexPosition(index, position);

			if (isThick())
				UpdateQuad();
		}

		public void SetPoints(Vector2f startPosition, Vector2f endPosition)
		{
			ModifyVertexPosition(0, startPosition);
			ModifyVertexPosition(1, endPosition);

			if (isThick()) UpdateQuad();
		}

		public Vector2f GetPoint(uint index)
		{
			if (index > 1) return new Vector2f(0f, 0f);
			return m_vertices[index].Position;
		}

		public FloatRect GetLocalBounds()
		{
			FloatRect box = new FloatRect();
			if (isThick())
			{
				float minX, maxX, minY, maxY;
				minX = maxX = m_quad[0].Position.X;
				minY = maxY = m_quad[0].Position.Y;
				for (uint v = 1u; v < 4; ++v)
				{
					minX = Math.Min(minX, m_quad[v].Position.X);
					maxX = Math.Max(maxX, m_quad[v].Position.X);
					minY = Math.Min(minY, m_quad[v].Position.Y);
					maxY = Math.Max(maxY, m_quad[v].Position.Y);
				}
				box.Left = minX;
				box.Top = minY;
				box.Width = maxX - minX;
				box.Height = maxY - minY;
			}
			else
			{
				box.Left = Math.Min(m_vertices[0].Position.X, m_vertices[1].Position.X);
				box.Top = Math.Min(m_vertices[0].Position.Y, m_vertices[1].Position.Y);
				box.Width = Math.Max(m_vertices[0].Position.X, m_vertices[1].Position.X) - box.Left;
				box.Height = Math.Max(m_vertices[0].Position.Y, m_vertices[1].Position.Y) - box.Top;
			}
			return box;
		}

		public FloatRect GetGlobalBounds()
		{
			FloatRect box = new FloatRect();
			if (isThick())
			{
				Transform transform = GetTransform();
				Vector2f transformedPosition0 = transform.TransformPoint(m_quad[0].Position);
				float minX, maxX, minY, maxY;
				minX = maxX = transformedPosition0.X;
				minY = maxY = transformedPosition0.Y;
				for (uint v = 1u; v < 4; ++v)
				{
					Vector2f transformedPosition = transform.TransformPoint(m_quad[v].Position);
					minX = Math.Min(minX, transformedPosition.X);
					maxX = Math.Max(maxX, transformedPosition.X);
					minY = Math.Min(minY, transformedPosition.Y);
					maxY = Math.Max(maxY, transformedPosition.Y);
				}
				box.Left = minX;
				box.Top = minY;
				box.Width = maxX - minX;
				box.Height = maxY - minY;
			}
			else
			{
				Vector2f transformedStartPosition = GetTransform().TransformPoint(m_vertices[0].Position);
				Vector2f transformedEndPosition = GetTransform().TransformPoint(m_vertices[1].Position);
				box.Left = Math.Min(transformedStartPosition.X, transformedEndPosition.X);
				box.Top = Math.Min(transformedStartPosition.Y, transformedEndPosition.Y);
				box.Width = Math.Max(transformedStartPosition.X, transformedEndPosition.X) - box.Left;
				box.Height = Math.Max(transformedStartPosition.Y, transformedEndPosition.Y) - box.Top;
			}
			return box;
		}



		public PointIndex GetStartIndex()
		{
			return PointIndex.Start;
		}

		public PointIndex GetEndIndex()
		{
			return PointIndex.End;
		}

		public Color GetColor()
		{
			return m_vertices[0].Color;
		}

		public void SetColor(Color color)
		{
			//m_color = color;
			ModifyVertexColor(0u, color);
			ModifyVertexColor(1u, color);
			ModifyQuadColor(0u, color);
			ModifyQuadColor(1u, color);
			ModifyQuadColor(2u, color);
			ModifyQuadColor(3u, color);
		}

		public void SetTexture(Texture texture, Vector2f size)
		{
			m_texture = texture;
			m_textureRect = new FloatRect(new Vector2f(0f, 0f), size);
			UpdateQuad();
		}

		public void SetTexture()
		{
			m_texture = null;
		}

		public Texture GetTexture()
		{
			return m_texture;
		}

		public void SetTextureRect(FloatRect textureRect)
		{
			m_textureRect = textureRect;
			UpdateQuad();
		}

		public FloatRect GetTextureRect()
		{
			return m_textureRect;
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= GetTransform();
			states.Texture = null;
			if (isThick())
			{
				if (m_texture != null) states.Texture = m_texture;
				target.Draw(m_quad, states);
			}
			else target.Draw(m_vertices, states);
		}

		bool isThick()
		{
			return (m_thickness >= thicknessEpsilon || m_thickness <= -thicknessEpsilon);
		}

		void SetThickness(float thickness)
		{
			m_thickness = (float)thickness;

			if (isThick()) UpdateQuad();
		}

		void ModifyVertexPosition(uint index, Vector2f position)
		{
			var vertex = m_vertices[index];
			vertex.Position = position;
			m_vertices[index] = vertex;
		}
		void ModifyQuadTexCoords(uint index, Vector2f position)
		{
			var vertex = m_quad[index];
			vertex.TexCoords = position;
			m_quad[index] = vertex;
		}
		void ModifyQuadPosition(uint index, Vector2f position)
		{
			var vertex = m_quad[index];
			vertex.Position = position;
			m_quad[index] = vertex;
		}
		void ModifyVertexColor(uint index, Color color)
		{
			var vertex = m_vertices[index];
			vertex.Color = color;
			m_vertices[index] = vertex;
		}
		void ModifyQuadColor(uint index, Color color)
		{
			var vertex = m_quad[index];
			vertex.Color = color;
			m_quad[index] = vertex;
		}


		void UpdateQuad()
		{
			Vector2f lineVector = m_vertices[0].Position - m_vertices[1].Position;
			float lineLength = (float)Math.Sqrt(lineVector.X * lineVector.X + lineVector.Y * lineVector.Y);
			Vector2f unitVector = lineVector / lineLength ;
		    Vector2f unitNormalVector = new Vector2f(unitVector.Y, -unitVector.X);
			Vector2f normalVector = unitNormalVector * m_thickness / 2f;

			ModifyQuadPosition(0u, m_vertices[0].Position - normalVector);
			ModifyQuadPosition(1u, m_vertices[1].Position - normalVector);
			ModifyQuadPosition(2u, m_vertices[1].Position + normalVector);
			ModifyQuadPosition(3u, m_vertices[0].Position + normalVector);

			ModifyQuadTexCoords(0u, new Vector2f(m_textureRect.Left, m_textureRect.Top ));
			ModifyQuadTexCoords(1u, new Vector2f(m_textureRect.Left + m_textureRect.Width, m_textureRect.Top));
			ModifyQuadTexCoords(2u, new Vector2f(m_textureRect.Left + m_textureRect.Width, m_textureRect.Top + m_textureRect.Height));
			ModifyQuadTexCoords(3u, new Vector2f(m_textureRect.Left, m_textureRect.Top + m_textureRect.Height));
		}


	}
}
