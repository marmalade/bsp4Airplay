#pragma once
#include <IwManaged.h>
#include <IwGx.h>

namespace Bsp4Airplay
{
	enum b4aCollisionResult
	{
		COLLISION_NONE=0,
		COLLISION_SOMEWHERE=1,
		COLLISION_ATSTART=3 //COLLISION_ATSTART|COLLISION_SOMEWHERE==COLLISION_ATSTART
	};

	struct Cb4aTraceContext
	{
		CIwVec3 from;
		CIwVec3 to;
		CIwVec3 collisionNormal;
		iwfixed collisionPlaneD;
	};

	class Ib4aCollider
	{
		public:

		//Constructor
		inline Ib4aCollider(){};
		//Desctructor
		inline virtual ~Ib4aCollider(){};

		virtual b4aCollisionResult TraceLine(Cb4aTraceContext& context) const =0;
		virtual b4aCollisionResult TraceSphere(int32 sphere, Cb4aTraceContext& context) const=0;
	};
	class Ib4aColliderContainer
	{
	public:
		inline Ib4aColliderContainer() {}
		inline virtual ~Ib4aColliderContainer() {}

		virtual void AddCollider(Ib4aCollider*)=0;
	};
	
}