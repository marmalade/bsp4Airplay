#include "Ib4aProjection.h"
#include "b4aLevelVBSubcluster.h"
#include "b4aLevelVB.h"
#include "b4aPlane.h"

using namespace Bsp4Airplay;
void Cb4aFlashlightProjectionVertex::Lerp(Cb4aFlashlightProjectionVertex*dst, const Cb4aFlashlightProjectionVertex& v0, int32 d0, const Cb4aFlashlightProjectionVertex& v1, int32 d1)
{
	int32 total = (int32)d0-d1;
	dst->pos.x = (int16)( ((int32)v1.pos.x*d0-(int32)v0.pos.x*d1)/total );
	dst->pos.y = (int16)( ((int32)v1.pos.y*d0-(int32)v0.pos.y*d1)/total );
	dst->pos.z = (int16)( ((int32)v1.pos.z*d0-(int32)v0.pos.z*d1)/total );
	dst->uv0.x = (int16)( ((int32)v1.uv0.x*d0-(int32)v0.uv0.x*d1)/total );
	dst->uv0.y = (int16)( ((int32)v1.uv0.y*d0-(int32)v0.uv0.y*d1)/total );
	/*dst->uv1.x = (int32)( (((float)v1.uv1.x)*d0-((float)v0.uv1.x)*d1)/total );
	dst->uv1.y = (int32)( (((float)v1.uv1.y)*d0-((float)v0.uv1.y)*d1)/total );
	dst->uv1.z = (int32)( (((float)v1.uv1.z)*d0-((float)v0.uv1.z)*d1)/total );*/
}
Cb4aFlashlightProjection::Cb4aFlashlightProjection()
{
	near = 8;
}
//inline int32 projDot(const CIwVec3& longVector,const CIwVec3& normal)
//	{
//		return (int32)((
//        (longVector.x) * normal.x +
//        (longVector.y) * normal.y +
//        (longVector.z) * normal.z +
//        0)>>12);
//	}
//inline int32 projPlaneDist(const CIwVec3& a,const Cb4aPlane& b)
//{
//	return projDot(a,b.v)-b.k;
//}
inline CIwVec3 projVec3(const CIwSVec3&v, int shift = IW_GEOM_POINT)
{
	return CIwVec3(((int32)v.x)<<shift,((int32)v.y)<<shift,((int32)v.z)<<shift);
}
inline CIwVec3 projVec3(const CIwVec3&v, int shift = IW_GEOM_POINT)
{
	return CIwVec3(((int32)v.x)<<shift,((int32)v.y)<<shift,((int32)v.z)<<shift);
}
int32 projIW_FIXED_MUL3(int32 a,int32 b,int32 c,int32 d,int32 e,int32 f)
{
	return a*d+b*e+c*f;
}
CIwVec3 projTransposeTransformVec(const CIwMat& m, CIwSVec3 const &V) 
{
	return CIwVec3(
		projIW_FIXED_MUL3(m.m[0][0], m.m[0][1], m.m[0][2], V.x-m.t.x, V.y-m.t.y, V.z-m.t.z),
		projIW_FIXED_MUL3(m.m[1][0], m.m[1][1], m.m[1][2], V.x-m.t.x, V.y-m.t.y, V.z-m.t.z),
		projIW_FIXED_MUL3(m.m[2][0], m.m[2][1], m.m[2][2], V.x-m.t.x, V.y-m.t.y, V.z-m.t.z)
		);
}
void Cb4aFlashlightProjection::Add(Cb4aFlashlightProjectionFace& face,int plane)
{
	start:
	if (plane == 6)
	{
		CIwSVec2 proj;
		for (int i=0; i<3;++i)
		{
			const Cb4aFlashlightProjectionVertex & v = face.vertices[i];
			CIwVec3 uv1 = projTransposeTransformVec(matrix,v.pos);
			int r = 255*(whz.z-(uv1.z>>IW_GEOM_POINT))/whz.z;
			if (r > 255) 
				r = 255;
			if (r < 0) 
				r = 0;
			float k = 1;//2048.0f*whz.z/(float)uv1.z;
			proj.x =  (int16)( k*uv1.x/whz.x);
			proj.y =  (int16)( uv1.z/whz.z /*k*uv1.y/whz.y*/);
			//proj.x =  (int16)( ((float)whz.z)*v.uv1.x/((float)v.uv1.z)*4096.0f/whz.x);
			//proj.y =  (int16)( ((float)whz.z)*v.uv1.y/((float)v.uv1.z)*4096.0f/whz.y);
			proj.x += IW_GEOM_ONE/2;
			//proj.y += IW_GEOM_ONE/2;
			positions.push_back(face.vertices[i].pos);
			uv0.push_back(proj);
			CIwColour c;
			c.Set(r,r,r,255);
			col.push_back(c);
			indices.push_back((uint16)indices.size());
		}
		return;
	}
	int32 dist[3];
	for (int i=0;i<3;++i)
		dist[i] = b4aPlaneDist(CIwVec3(face.vertices[i].pos), frustum[plane]);
	//cull
	if (dist[0] <= 0 && dist[1] <= 0 && dist[2] <= 0)
		return;
	//continue
	if (dist[0] >= 0 && dist[1] >= 0 && dist[2] >= 0)
	{
		++plane;
		goto start;
	}
	//slice
	int ptrs[3];
	if (dist[0] < 0 && dist[1] >= 0)
	{
		ptrs[0] = 0;
		ptrs[1] = 1;
		ptrs[2] = 2;
	} 
	else if (dist[1] < 0 && dist[2] >= 0)
	{
		ptrs[0] = 1;
		ptrs[1] = 2;
		ptrs[2] = 0;
	}
	else 
	{
		ptrs[0] = 2;
		ptrs[1] = 0;
		ptrs[2] = 1;
	}
	if (dist[ptrs[1]] == 0)
	{
		Cb4aFlashlightProjectionVertex v;
		Cb4aFlashlightProjectionVertex::Lerp(&v,face.vertices[ptrs[2]], dist[ptrs[2]],face.vertices[ptrs[0]], dist[ptrs[0]]);
		face.vertices[ptrs[0]] = v;
		++plane;
		goto start;
	}
	Cb4aFlashlightProjectionVertex v01;
	Cb4aFlashlightProjectionVertex::Lerp(&v01,face.vertices[ptrs[0]], dist[ptrs[0]],face.vertices[ptrs[1]], dist[ptrs[1]]);
	if (dist[ptrs[2]] > 0)
	{
		Cb4aFlashlightProjectionVertex v20;
		Cb4aFlashlightProjectionVertex::Lerp(&v20,face.vertices[ptrs[2]], dist[ptrs[2]],face.vertices[ptrs[0]], dist[ptrs[0]]);
		
		Cb4aFlashlightProjectionFace face1;
		face1.vertices[0] = v20;
		face1.vertices[1] = v01;
		face1.vertices[2] = face.vertices[ptrs[1]];
		Add(face1,plane+1);
		face1.vertices[0] = v20;
		face1.vertices[1] = face.vertices[ptrs[1]];
		face1.vertices[2] = face.vertices[ptrs[2]];
		Add(face1,plane+1);
		return;
	}
	{
		Cb4aFlashlightProjectionVertex v12;
		Cb4aFlashlightProjectionVertex::Lerp(&v12,face.vertices[ptrs[1]], dist[ptrs[1]],face.vertices[ptrs[2]], dist[ptrs[2]]);
		
		Cb4aFlashlightProjectionFace face1;
		face1.vertices[0] = v01;
		face1.vertices[1] = face.vertices[ptrs[1]];
		face1.vertices[2] = v12;
		Add(face1,plane+1);
	}
}

void Cb4aFlashlightProjection::Add(Cb4aLevel* level, Cb4aLevelVertexBuffer* buffer, Cb4aLevelVBSubcluster* geometry)
{
	if (b4aIsBBoxIntersect(projectionBox, geometry->GetBBox()))
	{
		uint32 size = geometry->indices.size();
		Cb4aFlashlightProjectionFace face;
		for (uint i=0; i<size; i+=3)
		{
			face.vertices[0].pos = buffer->GetPosition(geometry->indices[i]);
			face.vertices[1].pos = buffer->GetPosition(geometry->indices[i+1]);
			face.vertices[2].pos = buffer->GetPosition(geometry->indices[i+2]);
			//face.vertices[0].uv1 = projTransposeTransformVec(matrix,face.vertices[0].pos);
			//face.vertices[1].uv1 = projTransposeTransformVec(matrix,face.vertices[1].pos);
			//face.vertices[2].uv1 = projTransposeTransformVec(matrix,face.vertices[2].pos);
			//if (face.vertices[0].uv1.z < near*IW_GEOM_ONE && face.vertices[1].uv1.z < near*IW_GEOM_ONE && face.vertices[2].uv1.z < near*IW_GEOM_ONE)
			//	continue;
			//if (face.vertices[0].uv1.z > whz.z*IW_GEOM_ONE && face.vertices[1].uv1.z > whz.z*IW_GEOM_ONE && face.vertices[2].uv1.z > whz.z*IW_GEOM_ONE)
			//	continue;

			//face.vertices[0].n = matrix.TransposeRotateVec(buffer->GetNormal(geometry->indices[i])).z;
			//face.vertices[1].n = matrix.TransposeRotateVec(buffer->GetNormal(geometry->indices[i+1])).z;
			//face.vertices[2].n = matrix.TransposeRotateVec(buffer->GetNormal(geometry->indices[i+2])).z;

			//if (face.vertices[0].n > 0 && face.vertices[1].n > 0 && face.vertices[2].n > 0)
			//	continue;

			face.vertices[0].uv0 = buffer->GetUV0(geometry->indices[i]);
			face.vertices[1].uv0 = buffer->GetUV0(geometry->indices[i+1]);
			face.vertices[2].uv0 = buffer->GetUV0(geometry->indices[i+2]);
			Add(face,0);


		}
	}
	else
	{
		//bool ff = true;
	}
}
void Cb4aFlashlightProjection::Flush()
{
	if (indices.empty())
		return;
	CIwMaterial* mat = IW_GX_ALLOC_MATERIAL();
	mat->SetTexture(texure);
	mat->SetAlphaMode(CIwMaterial::ALPHA_ADD);
	mat->SetModulateMode(CIwMaterial::MODULATE_R);
	//mat->SetBlendMode(CIwMaterial::BLEND_ADD);
	mat->SetDepthWriteMode(CIwMaterial::DEPTH_WRITE_DISABLED);
	mat->SetZDepthOfs(-4);
	mat->SetZDepthOfsHW(-4);
	IwGxSetMaterial(mat);

	IwGxSetVertStream(&positions[0], indices.size());
	IwGxSetUVStream(&uv0[0], 0);
	IwGxSetColStream(&col[0]);
	IwGxDrawPrims(IW_GX_TRI_LIST,&indices[0],indices.size());
}
void Cb4aFlashlightProjection::Clear()
{
	positions.clear();
	uv0.clear();
	col.clear();
	indices.clear();
}

void Cb4aFlashlightProjection::Prepare(CIwTexture* tex, const CIwMat& mat, const CIwVec3 & _whz)
{
	texure = tex;
	matrix = mat;
	whz = _whz;

	projectionBox.m_Min = matrix.t;
	projectionBox.m_Max = matrix.t;
	CIwVec3 v;
	v = matrix.TransformVec(whz);
	projectionBox.BoundVec(&v);
	v = matrix.TransformVec(CIwVec3(-whz.x,-whz.y,whz.z));
	projectionBox.BoundVec(&v);
	v = matrix.TransformVec(CIwVec3(-whz.x,whz.y,whz.z));
	projectionBox.BoundVec(&v);
	v = matrix.TransformVec(CIwVec3(whz.x,-whz.y,whz.z));
	projectionBox.BoundVec(&v);

	//matrix.t = projVec3(matrix.t,0);

	CIwVec3 n;
	n = matrix.RotateVec(CIwVec3(0,0,IW_GEOM_ONE));
	frustum[0] = Cb4aPlane(n,n*matrix.t+near);
	n = matrix.RotateVec(CIwVec3(0,0,-IW_GEOM_ONE));
	frustum[1] = Cb4aPlane(n,n*matrix.t-whz.z);
	n.x = -whz.z;
	n.y = 0;
	n.z = whz.x;
	n.Normalise(); n = matrix.RotateVec(n);
	frustum[2] = Cb4aPlane(n,n*matrix.t);
	n.x = whz.z;
	n.y = 0;
	n.z = whz.x;
	n.Normalise(); n = matrix.RotateVec(n);
	frustum[3] = Cb4aPlane(n,n*matrix.t);
	n.x = 0;
	n.y = -whz.z;
	n.z = whz.y;
	n.Normalise(); n = matrix.RotateVec(n);
	frustum[4] = Cb4aPlane(n,n*matrix.t);
	n.x = 0;
	n.y = whz.z;
	n.z = whz.y;
	n.Normalise(); n = matrix.RotateVec(n);
	frustum[5] = Cb4aPlane(n,n*matrix.t);

	Clear();
}

Cb4aSkyProjection::Cb4aSkyProjection()
{
}
void Cb4aSkyProjection::Add(Cb4aLevel* level, Cb4aLevelVertexBuffer* buffer, Cb4aLevelVBSubcluster* geometry)
{

}
void Cb4aSkyProjection::Flush()
{
}

