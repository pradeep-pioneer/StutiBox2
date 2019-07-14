import React from "react";
import Slider from "react-slick";
import micky1 from '../Images/micky-1.jpg'
import micky2 from '../Images/micky-2.jpg'
import micky3 from '../Images/micky-3.jpg'
import micky4 from '../Images/micky-4.jpg'
import all1 from '../Images/all-1.jpg'
import all2 from '../Images/all-2.jpg'
import jasmine1 from '../Images/jasmine-1.jpg'
import jasmine2 from '../Images/jasmine-2.jpg'
import jasmine3 from '../Images/jasmine-3.jpg'
import jasmine4 from '../Images/jasmine-4.jpg'
import jasmine5 from '../Images/jasmine-5.jpg'
import mickyMe1 from '../Images/micky-me-1.jpg'
import mickySumit1 from '../Images/micky-sumit-1.jpg'
import mickySumit2 from '../Images/micky-sumit-2.jpg'
const pictures = [
  micky1, micky2, micky3, micky4,
  all1, all2,
  jasmine1, jasmine2, jasmine3, jasmine4, jasmine5,
  mickyMe1,
  mickySumit1, mickySumit2
]
export class PhotoSlider extends React.Component {
  render() {
    var settings = {
      dots: true,
      infinite: true,
      slidesToShow: 3,
      slidesToScroll: 1,
      autoplay: true,
      speed: 2000,
      autoplaySpeed: 2000,
      cssEase: "linear"
    };
    return (
      <Slider {...settings} autoplay>
        {pictures.map(picture=>{
          return(
            <div id={picture} key={picture}>
              <img style={{height:"40rem", margin: "0 auto"}} src={picture} />
            </div>
          )
        })}
      </Slider>
    );
  }
}