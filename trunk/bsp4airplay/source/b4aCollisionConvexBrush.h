#pragma once
#include <IwManaged.h>
#include <IwGx.h>
#include "Bsp4Airplay.h"
#include "Ib4aCollider.h"
#include "b4aPlane.h"

namespace Bsp4Airplay
{
	class Cb4aCollisionConvexBrush : public CIwManaged, public Ib4aCollider
	{
		CIwArray<Cb4aCollisionMeshSoupPlane> planes;
		public:
		//Declare managed class
		IW_MANAGED_DECLARE(Cb4aCollisionConvexBrush);

		//Constructor
		Cb4aCollisionConvexBrush();
		//Desctructor
		virtual ~Cb4aCollisionConvexBrush();

		virtual void  Serialise ();

		virtual b4aCollisionResult TraceLine(Cb4aTraceContext& context) const ;
		virtual b4aCollisionResult TraceSphere(int32 r, Cb4aTraceContext& context) const;
		
		// ---- Text resources ----
#ifdef IW_BUILD_RESOURCES
		// function invoked by the text parser when parsing attributes for objects of this type
		virtual bool ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName);

		// function invoked by the text parser when the object definition end is encountered
		virtual void ParseClose(CIwTextParserITX* pParser);
#endif
	};
}
