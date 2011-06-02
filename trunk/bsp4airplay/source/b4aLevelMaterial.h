#pragma once
#include <IwResource.h>
#include <IwModel.h>

namespace Bsp4Airplay
{
	class Cb4aLevelMaterial;
	class Cb4aLevel;

#ifdef IW_BUILD_RESOURCES
	void* Cb4aLevelMaterialFactory();

	class ParseLevelMaterial: public CIwManaged
	{
		Cb4aLevelMaterial* _this;
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

	class Cb4aLevelMaterial
	{
#ifdef IW_BUILD_RESOURCES
		friend class ParseLevelMaterial;
#endif
	public:
		uint32 texture0Hash;
		uint32 texture1Hash;
		CIwTexture* texture0;
		CIwTexture* texture1;
		bool transparent;
		bool sky;
		bool texturesResolved;
	public:

		//Constructor
		Cb4aLevelMaterial();
		//Desctructor
		~Cb4aLevelMaterial();

		void Serialise ();
		void Bind(Cb4aLevel* l);
	};

}