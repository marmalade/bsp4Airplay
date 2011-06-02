#pragma once
#include <IwResource.h>
#include <IwTexture.h>
#include <IwGx.h>

namespace Bsp4Airplay
{
	class Cb4aLevelVBSubcluster;
	class Cb4aLevel;
#ifdef IW_BUILD_RESOURCES
	void* Cb4aLevelVBSubclusterFactory();

	class ParseLevelVBSubcluster: public CIwManaged
	{
		Cb4aLevelVBSubcluster* _this;
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

	class Cb4aLevelVBSubcluster
	{
#ifdef IW_BUILD_RESOURCES
		friend class ParseLevelVBSubcluster;
#endif
	public:
		CIwArray<uint16> indices;
		CIwBBox bbox;
		CIwSphere sphere;
		uint32 material;
		uint32 vb;
	public:

		//Constructor
		Cb4aLevelVBSubcluster();
		//Desctructor
		~Cb4aLevelVBSubcluster();

		void  Serialise ();

		//void Render(Cb4aLevel*);

		bool IsVisible() const;

		inline const CIwArray<uint16> & GetIndices () const { return indices;};
		inline uint32 GetMaterial () const { return material;};
		inline const CIwBBox & GetBBox() const {return bbox;}
		inline const CIwSphere & GetSphere() const {return sphere;}
	};

}