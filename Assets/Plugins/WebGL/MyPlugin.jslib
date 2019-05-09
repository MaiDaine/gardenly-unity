mergeInto(LibraryManager.library, {
  save: function(elems) {
    ReactUnityWebGL.save(Pointer_stringify(elems));
  },
  unsavedWork: function(state) {
	ReactUnityWebGL.unsavedWork(state);
  },
  query: function(payload) {
	ReactUnityWebGL.query(Pointer_stringify(payload));
  }
});
