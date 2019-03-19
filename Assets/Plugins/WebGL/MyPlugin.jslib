mergeInto(LibraryManager.library, {
  SaveScene: function(elems) {
    ReactUnityWebGL.SaveScene(Pointer_stringify(elems));
  },
  UnsavedDataCheck: function(result) {
		ReactUnityWebGL.UnsavedDataCheck(result);
	}
});
