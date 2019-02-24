mergeInto(LibraryManager.library, {

  PreSaveScene: function (nbElems) {
    ReactUnityWebGL.SaveScene(nbElems);
  },

  SaveScene: function(elems) {
    ReactUnityWebGL.SaveScene(Pointer_stringify(elems));
  },

  UnsavedDataCheck: function(result) {
		ReactUnityWebGL.UnsavedDataCheck(result);
	}
});
