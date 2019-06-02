import * as React from 'react'
import * as ReactDOM from 'react-dom'
import Home from './Pages/Home'
//import './Styles/_main.scss';
//import './Styles/antMotionStyle.less'

ReactDOM.render(<Home/>,document.getElementById('react-app'))

if (module.hot) {
    module.hot.accept()
}