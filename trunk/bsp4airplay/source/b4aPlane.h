#pragma once

#include <iwgeom.h>

namespace Bsp4Airplay
{

	struct Cb4aPlane
	{
		CIwVec3 v;
		iwfixed k;

		inline Cb4aPlane(){}
		inline Cb4aPlane(const CIwVec3& vv, iwfixed kk):v(vv),k(kk){}
	};

	//inline int32 b4aDot(const CIwSVec3& a,const CIwSVec3& b)
	//{
	//	return (int32)a.x*(int32)b.x+(int32)a.y*(int32)b.y+(int32)a.z*(int32)b.z;
	//}
	//inline int32 b4aPlaneDist(const CIwSVec3& a,const CIwPlane& b)
	//{
	//	return b4aDot(b.v,a)-b.k;
	//}
	//inline CIwSVec3 b4aLerp(const CIwSVec3& a,const CIwSVec3& b,int32 adist, int32 bdist)
	//{
	//	int32 total = adist-bdist;
	//	if (total == 0)
	//		return a;
	//	while (total < -65536 || total > 65536)
	//	{
	//		total >>= 1;
	//		adist >>= 1;
	//		bdist >>= 1;
	//	}
	//	return CIwSVec3(
	//		(int16)((adist*b.x - bdist*a.x)/total),
	//		(int16)((adist*b.y - bdist*a.y)/total),
	//		(int16)((adist*b.z - bdist*a.z)/total));
	//}
	//const int32 b4aCollisionEpsilon=4096;

	inline int32 b4aDot(const CIwVec3& longVector,const CIwVec3& normal)
	{
		return (int32)((
        (longVector.x>>5) * normal.x +
        (longVector.y>>5) * normal.y +
        (longVector.z>>5) * normal.z +
        0)>>7);
	}
	inline int32 b4aPlaneDist(const CIwVec3& a,const Cb4aPlane& b)
	{
		//return (int32)(a.x>>12)*(int32)b.v.x+(int32)(a.y>>12)*(int32)b.v.y+(int32)(a.z>>12)*(int32)b.v.z - b.k;
		return b4aDot(a,b.v)-b.k;
	}
	inline CIwVec3 b4aLerp(const CIwVec3& a,const CIwVec3& b,int32 adist, int32 bdist)
	{
		int32 total = adist-bdist;
		if (total == 0)
			return a;
		/*while (total < -(1<<7) || total > (1<<7))
		{
			total >>= 1;
			adist >>= 1;
			bdist >>= 1;
		}*/
		return CIwVec3(
			(int32)(((int64)adist*b.x - (int64)bdist*a.x)/total),
			(int32)(((int64)adist*b.y - (int64)bdist*a.y)/total),
			(int32)(((int64)adist*b.z - (int64)bdist*a.z)/total));
	}
	const int32 b4aCollisionEpsilon=4096;

	typedef iwfixed (*PlaneDistanceCalculator)(const CIwVec3 & viewer, const Cb4aPlane& plane);

	struct Cb4aCollisionMeshSoupPlane
	{
		Cb4aPlane plane;
		PlaneDistanceCalculator calc;

		inline iwfixed Calculate(const CIwVec3 & viewer) const
		{
			return calc(viewer,plane);
		}
	};

	inline iwfixed GenericPlaneDistanceCalculator(const CIwVec3 & viewer, const Cb4aPlane& plane)	{	return b4aPlaneDist(viewer,plane);	}
	inline iwfixed PlaneDistanceCalculatorX(const CIwVec3 & viewer, const Cb4aPlane& plane) { return viewer.x-plane.k; }
	inline iwfixed PlaneDistanceCalculatorY(const CIwVec3 & viewer, const Cb4aPlane& plane) { return viewer.y-plane.k; }
	inline iwfixed PlaneDistanceCalculatorZ(const CIwVec3 & viewer, const Cb4aPlane& plane) { return viewer.z-plane.k; }
	inline iwfixed PlaneDistanceCalculator_X(const CIwVec3 & viewer, const Cb4aPlane& plane) { return -viewer.x-plane.k; }
	inline iwfixed PlaneDistanceCalculator_Y(const CIwVec3 & viewer, const Cb4aPlane& plane) { return -viewer.y-plane.k; }
	inline iwfixed PlaneDistanceCalculator_Z(const CIwVec3 & viewer, const Cb4aPlane& plane) { return -viewer.z-plane.k; }

	inline PlaneDistanceCalculator GetDistanceCalculator(const Cb4aPlane& plane)
	{
		if (plane.v == CIwVec3(4096,0,0))
			return PlaneDistanceCalculatorX;
		else if (plane.v == CIwVec3(0,4096,0))
			return PlaneDistanceCalculatorY;
		else if (plane.v == CIwVec3(0,0,4096))
			return PlaneDistanceCalculatorZ;
		else if (plane.v == CIwVec3(-4096,0,0))
			return PlaneDistanceCalculator_X;
		else if (plane.v == CIwVec3(0,-4096,0))
			return PlaneDistanceCalculator_Y;
		else if (plane.v == CIwVec3(0,0,-4096))
			return PlaneDistanceCalculator_Z;
		return GenericPlaneDistanceCalculator;
	}

	inline bool b4aIsBBoxIntersect(const CIwBBox& a,const CIwBBox& b)
	{
		return (a.m_Min.x <= b.m_Max.x) && (a.m_Min.y <= b.m_Max.y) && (a.m_Min.z <= b.m_Max.z)
			&& (b.m_Min.x <= a.m_Max.x) && (b.m_Min.y <= a.m_Max.y) && (b.m_Min.z <= a.m_Max.z);
	}
}