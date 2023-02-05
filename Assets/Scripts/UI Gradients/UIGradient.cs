﻿//Taken from https://github.com/azixMcAze/Unity-UIGradient with credit
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class UIGradient : BaseMeshEffect {
	public Color m_color1 = Color.white;
	public Color m_color2 = Color.white;
	[Range(-180f, 180f)] public float m_angle;
	public bool m_ignoreRatio = true;

	public override void ModifyMesh(VertexHelper vh) {
		if (!enabled) return;
		var rect = graphic.rectTransform.rect;
		var dir = UIGradientUtils.RotationDir(m_angle);

		if (!m_ignoreRatio)
			dir = UIGradientUtils.CompensateAspectRatio(rect, dir);

		var localPositionMatrix = UIGradientUtils.LocalPositionMatrix(rect, dir);

		var vertex = default(UIVertex);
		for (var i = 0; i < vh.currentVertCount; i++) {
			vh.PopulateUIVertex(ref vertex, i);
			var localPosition = localPositionMatrix * vertex.position;
			vertex.color *= Color.Lerp(m_color2, m_color1, localPosition.y);
			vh.SetUIVertex(vertex, i);
		}
	}
}