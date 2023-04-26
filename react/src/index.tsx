import { useGlobals, useReactiveValue } from '@reactunity/renderer';
import { render } from '@reactunity/renderer';
import './index.scss';

import { MemoryRouter, Route, Routes, useNavigate } from 'react-router';
import { useEffect } from 'react';

import Menu from './menu';
import Dialogue from './dialogue';

export default function App() {
  const globals = useGlobals();
  const route = useReactiveValue(globals.route);

  const navigate = useNavigate();

  useEffect(() => {
    console.log(`Navigate to route: ${route}`);
    navigate(route);
  }, [route, navigate])

  return (
    <Routes>
      <Route path="/" element={<view />} />
      <Route path="/menu" element={<Menu />} />
      <Route path="/dialogue" element={<Dialogue />} />
    </Routes>
  );
}

render(
  <MemoryRouter>
    <App />
  </MemoryRouter>
);
