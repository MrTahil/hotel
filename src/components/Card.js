import React from 'react'

export default function Card(props) {
  return (
    <div>




<div className="card" style={{width: "18rem"}}>
  <img className="card-img-top" src={props.rooms} alt="Card image cap"/>
  <div className="card-body">
    <h5 className="card-title">{props.pizza.name}</h5>
    <p className="card-text">Gluténmentes: {props.pizza.isGlutenFree ? "igen" : "nem"}</p>
    <a href="#" classNameName="btn btn-warning">Részletek</a>
  </div>
</div>



    </div>
  )
}
