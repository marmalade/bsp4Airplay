#pragma once
#include <IwManaged.h>
#include <IwGx.h>
#include "Bsp4Airplay.h"
#include "Ib4aCollider.h"

namespace Bsp4Airplay
{
	class Cb4aCollisionMeshSoup;


	struct Cb4aCollisionMeshSoupFace
	{
		int start;
		int num;

		b4aCollisionResult TraceLine(const Cb4aCollisionMeshSoup& soup, Cb4aTraceContext& context) const;
		b4aCollisionResult TraceSphere(const Cb4aCollisionMeshSoup& soup, int32 r, Cb4aTraceContext& context) const;
	};
	class Cb4aCollisionMeshSoup : public CIwManaged, public Ib4aCollider
	{
		friend struct Cb4aCollisionMeshSoupFace;
		CIwArray<Cb4aCollisionMeshSoupFace> faces;
		CIwArray<Cb4aCollisionMeshSoupPlane> planes;
		public:
		//Declare managed class
		IW_MANAGED_DECLARE(Cb4aCollisionMeshSoup);

		//Constructor
		Cb4aCollisionMeshSoup();
		//Desctructor
		virtual ~Cb4aCollisionMeshSoup();

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