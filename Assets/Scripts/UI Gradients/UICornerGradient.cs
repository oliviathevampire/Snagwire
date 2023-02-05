//Taken from https://github.com/azixMcAze/Unity-UIGradient with credit
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/4 Corners Gradient")]
public class UICornersGradient : BaseMeshEffect {
	public Color m_topLeftColor = Color.white;
	public Color m_topRightColor = Color.white;
	public Color m_bottomRightColor = Color.white;
	public Color m_bottomLeftColor = Color.white;

	public override void ModifyMesh(VertexHelper vh) {
		if (!enabled) return;
		var rect = graphic.rectTransform.rect;
		var localPositionMatrix = UIGradientUtils.LocalPositionMatrix(rect, Vector2.right);

		var vertex = default(UIVertex);
		for (var i = 0; i < vh.currentVertCount; i++) {
			vh.PopulateUIVertex(ref vertex, i);
			var normalizedPosition = localPositionMatrix * vertex.position;
			vertex.color *= UIGradientUtils.Bilerp(m_bottomLeftColor, m_bottomRightColor, m_topLeftColor,
				m_topRightColor, normalizedPosition);
			vh.SetUIVertex(vertex, i);
		}
	}
}