#pragma once
#include <IwResource.h>
#include <IwModel.h>
#include "b4aColliderList.h"

namespace Bsp4Airplay
{
	class Cb4aLeaf;
	class Cb4aLevel;
	struct Cb4aTraceContext;
#ifdef IW_BUILD_RESOURCES
	void* Cb4aLeafFactory();

	class ParseLeaf: public CIwManaged, public Ib4aColliderContainer
	{
		Cb4aLeaf* _this;
		public:
		virtual void AddCollider(Ib4aCollider*);
		// ---- Text resources ----
		// Parse from text file: start block.
		virtual void  ParseOpen (CIwTextParserITX *pParser);

		// function invoked by the text parser when parsing attributes for objects of this type
		virtual bool ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName);

		// function invoked by the text parser when the object definition end is encountered
		virtual void ParseClose(CIwTextParserITX* pParser);
	};
#endif

	class Cb4aLeaf:public Ib4aColliderContainer
	{
#ifdef IW_BUILD_RESOURCES
		friend class ParseLeaf;
#endif
	public:
		CIwBBox bbox;
		CIwSphere sphere;
		CIwArray<int32> visible_leaves;
		CIwArray<int32> visible_clusters;
		Cb4aColliderList colliders;

		uint32 lastVisibleOnFrame;
	public:

		//Constructor
		Cb4aLeaf();
		//Desctructor
		~Cb4aLeaf();

		void  Serialise ();
		virtual void AddCollider(Ib4aCollider*);

		void Render(Cb4aLevel*);
		void RenderProjection(Cb4aLevel*,CIwTexture* tex, const CIwMat& view, const CIwVec3& whz);
		b4aCollisionResult TraceLine(const Cb4aLevel*, Cb4aTraceContext& context) const;
		b4aCollisionResult TraceSphere(const Cb4aLevel*, int32 sphere, Cb4aTraceContext& context) const;

		inline void SetVisible(uint32 frame) {lastVisibleOnFrame=frame;}
		inline uint32 GetVisible() const {return lastVisibleOnFrame;}
		inline const CIwBBox & GetBBox() const {return bbox;}
		inline const CIwSphere & GetSphere() const {return sphere;}
	};

}