Vertex<T: ℝ = f64> struct {
  position : Vector3<T>(0)
  normal   : Vector3<T>(0) // aka direction

  from (position: Vector3<T>) => Vertex<T>(position, Vector::from(0))
  
  interpolate (other: Vertex<T>, t: T) => Vertex {
    position : position.lerp(other.position, t),
    normal   : normal.lerp(other.normal, t)
  }
}