mergeInto(LibraryManager.library, {
  SaveScene: function(elems) {
    ReactUnityWebGL.SaveScene(Pointer_stringify(elems));
  },
  SetUnsavedWorkState: function(state) {
		ReactUnityWebGL.SetUnsavedWorkState(state);
	}
});
