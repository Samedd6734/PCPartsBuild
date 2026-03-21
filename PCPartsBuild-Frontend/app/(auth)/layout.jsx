'use client';

import React from 'react';

export default function AuthLayout({ children }) {
  return (
    <div className="relative min-h-screen w-full overflow-hidden bg-background-dark font-sans text-white antialiased">
      {/* Background Video - Auth Pages ONLY */}
      <div className="fixed inset-0 z-0 overflow-hidden">
        <video autoPlay className="h-full w-full object-cover brightness-[0.4]" loop muted playsInline>
          <source src="/Videos/Video1.webm" type="video/webm" />
          Your browser does not support the video tag.
        </video>
      </div>

      <div className="relative z-10 flex min-h-screen flex-col">
          {children}
      </div>
    </div>
  );
}
