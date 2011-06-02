#pragma once
#include <IwResource.h>
#include <IwGx.h>
#include "b4aPlane.h"
#include "Ib4aCollider.h"

namespace Bsp4Airplay
{
	
	class Cb4aNode
	{
	public:
		CIwBBox bbox;
		
		int32 plane;
		bool is_front_leaf;
		int32 front;
		bool is_back_leaf;
		int32 back;

		//Constructor
		Cb4aNode();
		//Desctructor
		~Cb4aNode();

		void  Serialise ();

		inline const CIwBBox & GetBBox() const {return bbox;}

		bool WalkNode(const Cb4aLevel*l, const CIwVec3 & viewer, int32* nextNode) const;
		b4aCollisionResult TraceLine(const Cb4aLevel*, Cb4aTraceContext& context) const;
		b4aCollisionResult TraceSphere(const Cb4aLevel*, int32 r, Cb4aTraceContext& context) const;
	protected:
		b4aCollisionResult TraceFrontLine(const Cb4aLevel*, Cb4aTraceContext& context) const;
		b4aCollisionResult TraceBackLine(const Cb4aLevel*, Cb4aTraceContext& context) const;

		b4aCollisionResult TraceFrontSphere(const Cb4aLevel*, int32 sphere, Cb4aTraceContext& context) const;
		b4aCollisionResult TraceBackSphere(const Cb4aLevel*, int32 sphere, Cb4aTraceContext& context) const;
	};

#ifdef IW_BUILD_RESOURCES
	void* Cb4aNodeFactory();

	class ParseNode: public CIwManaged
	{
		Cb4aNode* _this;
	public:
		// ---- Text resources ----
		// Parse from text file: start block.
		virtual void  ParseOpen (CIwTextParserITX *pParser);

		// function invoked by the text parser when parsing attributes for objects of this type
		virtual bool ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName);

		// function invoked by the text parser when the object definition end is encountered
		virtual void ParseClose(CIwTextParserITX* pParser);
	};
#endif
}