using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Catalyst.Engine.Core;

namespace Catalyst.Engine.Data;

/// <summary>
/// Contains level data.
/// </summary>
public class Level<T> where T : ILevelObject
{
    private class LevelView : ILevelView
    {
        private readonly Level<T> level;
        
        public event EventHandler<ILevelObject>? ObjectInserted;
        public event EventHandler<ILevelObject>? ObjectRemoved;
        
        public LevelView(Level<T> level)
        {
            this.level = level;
            this.level.ObjectInserted += OnObjectInserted;
            this.level.ObjectRemoved += OnObjectRemoved;
        }
        
        ~LevelView()
        {
            level.ObjectInserted -= OnObjectInserted;
            level.ObjectRemoved -= OnObjectRemoved;
        }

        private void OnObjectInserted(object sender, T e)
        {
            ObjectInserted?.Invoke(this, e);
        }

        private void OnObjectRemoved(object sender, T e)
        {
            ObjectRemoved?.Invoke(this, e);
        }

        public IEnumerator<ILevelObject> GetEnumerator()
        {
            return level.Objects
                .Cast<ILevelObject>()
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public ILevelView View { get; }

    public IReadOnlyList<T> Objects => objects;

    private readonly List<T> objects;
    
    public event EventHandler<T>? ObjectInserted;
    public event EventHandler<T>? ObjectRemoved;

    public Level(IEnumerable<T> levelObjects)
    {
        objects = levelObjects.ToList();
        View = new LevelView(this);
    }
    
    public void InsertObject(T levelObject)
    {
        objects.Add(levelObject);
        ObjectInserted?.Invoke(this, levelObject);
    }
    
    public void RemoveObject(T levelObject)
    { 
        objects.Remove(levelObject);
        ObjectRemoved?.Invoke(this, levelObject);
    }
}